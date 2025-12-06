namespace Urban.AI.Application.UserManagement.Users.UpdateUser;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class UpdateUserHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IIdentityProvider _identityProvider;

    public UpdateUserHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IIdentityProvider identityProvider)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _identityProvider = identityProvider;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        var keycloakUpdateResult = await _identityProvider.UpdateUserAsync(
            user.IdentityId,
            request.UserToUpdate.FirstName,
            request.UserToUpdate.LastName,
            request.UserToUpdate.Email,
            cancellationToken);

        if (keycloakUpdateResult.IsFailure)
        {
            return Result.Failure(keycloakUpdateResult.Error);
        }

        user.UpdateBasicInfo(
            request.UserToUpdate.Email,
            request.UserToUpdate.FirstName,
            request.UserToUpdate.LastName);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
