namespace Urban.AI.Application.Incidents.GetLeaderIncidents;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Application.Incidents.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Incidents;
using Urban.AI.Domain.Leaders;
using Urban.AI.Domain.Users;
#endregion

internal sealed class GetLeaderIncidentsHandler : IQueryHandler<GetLeaderIncidentsQuery, LeaderIncidentsResponse>
{
    #region Constants
    private const string IncidentImagesBucket = "incident-images";
    private const int PresignedUrlExpirationHours = 24;
    #endregion

    #region Private Members
    private readonly IIncidentRepository _incidentRepository;
    private readonly ILeaderRepository _leaderRepository;
    private readonly IStorageService _storageService;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    #endregion

    public GetLeaderIncidentsHandler(
        IIncidentRepository incidentRepository,
        ILeaderRepository leaderRepository,
        IStorageService storageService,
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _incidentRepository = incidentRepository;
        _leaderRepository = leaderRepository;
        _storageService = storageService;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result<LeaderIncidentsResponse>> Handle(
        GetLeaderIncidentsQuery request,
        CancellationToken cancellationToken)
    {
        var userIdGuid = Guid.Parse(_userContext.IdentityId);

        var user = await _userRepository.GetByEmailAsync(_userContext.Email, cancellationToken);
        if (user is null) return Result.Failure<LeaderIncidentsResponse>(UserErrors.NotFound);

        var leader = await _leaderRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (leader is null) return Result.Failure<LeaderIncidentsResponse>(LeaderErrors.LeaderNotFound);

        var incidents = await _incidentRepository.GetByLeaderIdAsync(leader.Id, cancellationToken);

        var acceptedIncidents = new List<IncidentResponse>();
        var pendingIncidents = new List<IncidentResponse>();
        var rejectedIncidents = new List<IncidentResponse>();

        foreach (var incident in incidents)
        {
            var imageUrl = await GetImageUrlAsync(incident.ImagePath, cancellationToken);
            var incidentResponse = incident.ToResponse(imageUrl);

            switch (incident.Status)
            {
                case IncidentStatus.Accepted:
                    acceptedIncidents.Add(incidentResponse);
                    break;
                case IncidentStatus.Pending:
                    pendingIncidents.Add(incidentResponse);
                    break;
                case IncidentStatus.Rejected:
                    rejectedIncidents.Add(incidentResponse);
                    break;
            }
        }

        var response = new LeaderIncidentsResponse(
            acceptedIncidents.OrderByDescending(i => i.CreatedAt),
            pendingIncidents.OrderByDescending(i => i.CreatedAt),
            rejectedIncidents.OrderByDescending(i => i.CreatedAt));

        return Result.Success(response);
    }

    private async Task<string> GetImageUrlAsync(string imagePath, CancellationToken cancellationToken)
    {
        var file = File.CreateForGet(imagePath, imagePath, IncidentImagesBucket);
        var presignedUrlResult = await _storageService.GetPresignedUrl(file, PresignedUrlExpirationHours, cancellationToken);
        return presignedUrlResult.IsSuccess ? presignedUrlResult.Value : string.Empty;
    }
}
