namespace Urban.AI.Application.UserManagement.Users.AssignRolesToUser;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class AssignRolesToUserHandler : ICommandHandler<AssignRolesToUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IIdentityProvider _identityProvider;

    public AssignRolesToUserHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IIdentityProvider identityProvider)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _identityProvider = identityProvider;
    }

    public async Task<Result> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        var identityProviderAssignResult = await _identityProvider.AssignRolesToUserAsync(
            user.IdentityId,
            request.RolesToAssign.RoleNames,
            cancellationToken);

        if (identityProviderAssignResult.IsFailure)
        {
            return Result.Failure(identityProviderAssignResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
