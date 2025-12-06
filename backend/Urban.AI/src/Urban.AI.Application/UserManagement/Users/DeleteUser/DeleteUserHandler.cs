namespace Urban.AI.Application.UserManagement.Users.DeleteUser;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class DeleteUserHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IIdentityProvider _identityProvider;

    public DeleteUserHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IIdentityProvider identityProvider)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _identityProvider = identityProvider;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        var keycloakDeleteResult = await _identityProvider.DeleteUserAsync(user.IdentityId, cancellationToken);

        if (keycloakDeleteResult.IsFailure)
        {
            return Result.Failure(keycloakDeleteResult.Error);
        }

        _userRepository.Delete(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
