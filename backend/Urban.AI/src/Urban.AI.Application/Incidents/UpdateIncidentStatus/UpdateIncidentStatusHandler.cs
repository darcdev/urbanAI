namespace Urban.AI.Application.Incidents.UpdateIncidentStatus;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents;
#endregion

internal sealed class UpdateIncidentStatusHandler : ICommandHandler<UpdateIncidentStatusCommand>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncidentStatusHandler(
        IIncidentRepository incidentRepository,
        IUnitOfWork unitOfWork)
    {
        _incidentRepository = incidentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateIncidentStatusCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId, cancellationToken);
        if (incident is null)
        {
            return Result.Failure(IncidentErrors.NotFound);
        }

        switch (request.Status)
        {
            case IncidentStatus.Accepted:
                var priority = request.Priority ?? IncidentPriority.NotEstablished;
                incident.Accept(priority);
                break;

            case IncidentStatus.Rejected:
                incident.Reject();
                break;

            default:
                return Result.Failure(new Error("InvalidStatus", "Invalid status value"));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
