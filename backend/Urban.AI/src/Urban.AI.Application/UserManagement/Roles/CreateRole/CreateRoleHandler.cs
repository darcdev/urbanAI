namespace Urban.AI.Application.UserManagement.Roles.CreateRole;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
#endregion

internal sealed class CreateRoleHandler : ICommandHandler<CreateRoleCommand>
{
    private readonly IIdentityProvider _identityProvider;

    public CreateRoleHandler(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<Result> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var createResult = await _identityProvider.CreateRoleAsync(
            request.RoleToCreate.Name,
            request.RoleToCreate.Description,
            cancellationToken);

        if (createResult.IsFailure)
        {
            return Result.Failure(createResult.Error);
        }

        return Result.Success();
    }
}
