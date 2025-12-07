namespace Urban.AI.Application.Incidents.CreateIncident;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Email;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Application.Incidents.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Geography;
using Urban.AI.Domain.Incidents;
using Urban.AI.Domain.Leaders;
#endregion

internal sealed class CreateIncidentHandler : ICommandHandler<CreateIncidentCommand, IncidentResponse>
{
    #region Constants
    private const string IncidentImagesBucket = "incident-images";
    #endregion

    #region Private Members
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIncidentRepository _incidentRepository;
    private readonly ILeaderRepository _leaderRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IIncidentAnalysisService _incidentAnalysisService;
    private readonly IStorageService _storageService;
    private readonly IEmailService _emailService;
    #endregion

    public CreateIncidentHandler(
        IUnitOfWork unitOfWork,
        IIncidentRepository incidentRepository,
        ILeaderRepository leaderRepository,
        IMunicipalityRepository municipalityRepository,
        IIncidentAnalysisService incidentAnalysisService,
        IStorageService storageService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _incidentRepository = incidentRepository;
        _leaderRepository = leaderRepository;
        _municipalityRepository = municipalityRepository;
        _incidentAnalysisService = incidentAnalysisService;
        _storageService = storageService;
        _emailService = emailService;
    }

    public async Task<Result<IncidentResponse>> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        var assignmentResult = await AssignLeaderAndMunicipalityAsync(
            request.Request.Latitude,
            request.Request.Longitude,
            cancellationToken);

        if (assignmentResult.IsFailure)
        {
            return Result.Failure<IncidentResponse>(assignmentResult.Error);
        }

        var (leaderId, municipalityId) = assignmentResult.Value;

        var analysisResult = await _incidentAnalysisService.AnalyzeImageAsync(
            request.Request.ImageStream,
            cancellationToken);

        if (analysisResult.IsFailure)
        {
            return Result.Failure<IncidentResponse>(analysisResult.Error);
        }

        var imagePathResult = await SaveImageToStorageAsync(request.Request, cancellationToken);
        if (imagePathResult.IsFailure)
        {
            return Result.Failure<IncidentResponse>(imagePathResult.Error);
        }

        var (imagePath, imageFile) = imagePathResult.Value;

        var incident = CreateIncident(
            imagePath,
            request.Request,
            municipalityId,
            leaderId);

        incident.SetAnalysis(analysisResult.Value);

        _incidentRepository.Add(incident);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Request.CitizenEmail))
        {
            await SendRadicateNumberEmail(
                request.Request.CitizenEmail,
                incident.RadicateNumber,
                cancellationToken);
        }

        var imageUrl = await GetImageUrlAsync(imageFile, cancellationToken);

        return Result.Success(incident.ToResponse(imageUrl));
    }

    private async Task<Result<(Guid? LeaderId, Guid MunicipalityId)>> AssignLeaderAndMunicipalityAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken)
    {
        var nearestLeader = await _leaderRepository.GetNearestLeaderAsync(
            latitude,
            longitude,
            cancellationToken);

        if (nearestLeader is not null)
        {
            return Result.Success<(Guid?, Guid)>((nearestLeader.Id, nearestLeader.MunicipalityId));
        }

        var municipalities = await _municipalityRepository.GetAllAsync(cancellationToken);
        var municipalitiesWithCoordinates = municipalities
            .Where(m => m.Latitude.HasValue && m.Longitude.HasValue)
            .ToList();

        if (!municipalitiesWithCoordinates.Any())
        {
            return Result.Failure<(Guid?, Guid)>(IncidentErrors.MunicipalityNotFound);
        }

        var nearestMunicipality = municipalitiesWithCoordinates
            .Select(m => new
            {
                Municipality = m,
                Distance = CalculateHaversineDistance(
                    latitude,
                    longitude,
                    m.Latitude!.Value,
                    m.Longitude!.Value)
            })
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        if (nearestMunicipality is null)
        {
            return Result.Failure<(Guid?, Guid)>(IncidentErrors.MunicipalityNotFound);
        }

        return Result.Success<(Guid?, Guid)>((null, nearestMunicipality.Municipality.Id));
    }

    private async Task<Result<(string ImagePath, File ImageFile)>> SaveImageToStorageAsync(
        CreateIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var imageFilename = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFileName)}";
        var imagePath = $"{DateTime.UtcNow:yyyy/MM}/{imageFilename}";

        request.ImageStream.Position = 0;

        using var memoryStream = new MemoryStream();
        await request.ImageStream.CopyToAsync(memoryStream, cancellationToken);
        var imageContent = Convert.ToBase64String(memoryStream.ToArray());

        var file = File.CreateForSave(
            imageFilename,
            imageContent,
            imagePath,
            IncidentImagesBucket,
            request.ImageContentType);

        var saveFileResult = await _storageService.SaveFile(file, cancellationToken);
        if (saveFileResult.IsFailure)
        {
            return Result.Failure<(string, File)>(saveFileResult.Error);
        }

        return Result.Success((imagePath, file));
    }

    private static Incident CreateIncident(
        string imagePath,
        CreateIncidentRequest request,
        Guid municipalityId,
        Guid? leaderId)
    {
        var radicateNumber = GenerateRadicateNumber();
        var location = Location.Create(request.Latitude, request.Longitude);

        return Incident.Create(
            radicateNumber,
            imagePath,
            location,
            municipalityId,
            leaderId,
            request.CitizenEmail,
            request.AdditionalComment);
    }

    private async Task<string> GetImageUrlAsync(File file, CancellationToken cancellationToken)
    {
        var presignedUrlResult = await _storageService.GetPresignedUrl(file, 24, cancellationToken);
        return presignedUrlResult.IsSuccess ? presignedUrlResult.Value : string.Empty;
    }

    private static string GenerateRadicateNumber()
    {
        var uuid = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"INC-RA-{uuid}";
    }

    private static double CalculateHaversineDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians((double)(lat2 - lat1));
        var dLon = DegreesToRadians((double)(lon2 - lon1));

        var lat1Rad = DegreesToRadians((double)lat1);
        var lat2Rad = DegreesToRadians((double)lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1Rad) * Math.Cos(lat2Rad);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private async Task SendRadicateNumberEmail(string email, string radicateNumber, CancellationToken cancellationToken)
    {
        try
        {
            var subject = "Your Incident Radicate Number";
            var body = $@"
                <h2>Thank you for reporting an urban incident!</h2>
                <p>Your radicate number is: <strong>{radicateNumber}</strong></p>
                <p>You can use this number to track the status of your report.</p>
                <p>We appreciate your contribution to improving our urban environment.</p>
            ";

            await _emailService.SendEmailAsync(email, subject, body, cancellationToken);
        }
        catch (Exception)
        {
        }
    }
}