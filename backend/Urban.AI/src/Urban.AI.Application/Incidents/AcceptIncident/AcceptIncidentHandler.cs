namespace Urban.AI.Application.Incidents.AcceptIncident;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents;
#endregion

internal sealed class AcceptIncidentHandler : ICommandHandler<AcceptIncidentCommand>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptIncidentHandler(
        IIncidentRepository incidentRepository,
        IUnitOfWork unitOfWork)
    {
        _incidentRepository = incidentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AcceptIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId, cancellationToken);
        if (incident is null)
        {
            return Result.Failure(IncidentErrors.NotFound);
        }

        incident.Accept(request.Priority);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
