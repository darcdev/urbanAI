namespace Urban.AI.Application.Incidents.RejectIncident;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents;
#endregion

internal sealed class RejectIncidentHandler : ICommandHandler<RejectIncidentCommand>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectIncidentHandler(
        IIncidentRepository incidentRepository,
        IUnitOfWork unitOfWork)
    {
        _incidentRepository = incidentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId, cancellationToken);
        if (incident is null)
        {
            return Result.Failure(IncidentErrors.NotFound);
        }

        incident.Reject();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
