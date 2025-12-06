namespace Urban.AI.Application.UserManagement.Roles.DeleteRole;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
#endregion

internal sealed class DeleteRoleHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly IIdentityProvider _identityProvider;

    public DeleteRoleHandler(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await _identityProvider.DeleteRoleAsync(request.RoleName, cancellationToken);

        if (deleteResult.IsFailure)
        {
            return Result.Failure(deleteResult.Error);
        }

        return Result.Success();
    }
}
