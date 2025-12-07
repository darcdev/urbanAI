namespace Urban.AI.Application.Organizations.UpdateOrganization;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Organizations;
using Urban.AI.Domain.Users;
#endregion

internal sealed class UpdateOrganizationCommandHandler : ICommandHandler<UpdateOrganizationCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdWithDetailsAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationNotFound);
        }

        var user = await _userRepository.GetByIdAsync(organization.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(OrganizationErrors.UserNotFound);
        }

        user.UpdateBasicInfo(
            user.Email,
            request.Request.FirstName,
            request.Request.LastName);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
