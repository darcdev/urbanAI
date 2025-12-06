namespace Urban.AI.Application.UserManagement.Roles.UpdateRole;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
#endregion

internal sealed class UpdateRoleHandler : ICommandHandler<UpdateRoleCommand>
{
    private readonly IIdentityProvider _identityProvider;

    public UpdateRoleHandler(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var updateResult = await _identityProvider.UpdateRoleAsync(
            request.RoleName,
            request.RoleToUpdate.Description,
            cancellationToken);

        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        return Result.Success();
    }
}
