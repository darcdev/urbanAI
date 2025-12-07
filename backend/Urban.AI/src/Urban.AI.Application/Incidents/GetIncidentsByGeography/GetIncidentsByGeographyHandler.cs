namespace Urban.AI.Application.Incidents.GetIncidentsByGeography;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Application.Incidents.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Incidents;
#endregion

internal sealed class GetIncidentsByGeographyHandler : IQueryHandler<GetIncidentsByGeographyQuery, List<IncidentResponse>>
{
    #region Constants
    private const string IncidentImagesBucket = "incident-images";
    private const int PresignedUrlExpirationHours = 24;
    #endregion

    #region Private Members
    private readonly IIncidentRepository _incidentRepository;
    private readonly IStorageService _storageService;
    #endregion

    public GetIncidentsByGeographyHandler(
        IIncidentRepository incidentRepository,
        IStorageService storageService)
    {
        _incidentRepository = incidentRepository;
        _storageService = storageService;
    }

    public async Task<Result<List<IncidentResponse>>> Handle(
        GetIncidentsByGeographyQuery request,
        CancellationToken cancellationToken)
    {
        var incidents = await _incidentRepository.GetByGeographyAsync(
            request.DepartmentId,
            request.MunicipalityId,
            cancellationToken);

        var incidentResponses = new List<IncidentResponse>();

        foreach (var incident in incidents)
        {
            var imageUrl = await GetImageUrlAsync(incident.ImagePath, cancellationToken);
            var incidentResponse = incident.ToResponse(imageUrl);
            incidentResponses.Add(incidentResponse);
        }

        return Result.Success(incidentResponses);
    }

    private async Task<string> GetImageUrlAsync(string imagePath, CancellationToken cancellationToken)
    {
        var file = File.CreateForGet(imagePath, imagePath, IncidentImagesBucket);
        var presignedUrlResult = await _storageService.GetPresignedUrl(file, PresignedUrlExpirationHours, cancellationToken);
        return presignedUrlResult.IsSuccess ? presignedUrlResult.Value : string.Empty;
    }
}
