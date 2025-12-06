namespace Urban.AI.Application.UserManagement.Roles.GetRoleById;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Roles.Dtos;
using Urban.AI.Domain.Common.Abstractions;
#endregion

internal sealed class GetRoleByIdHandler : IQueryHandler<GetRoleByIdQuery, RoleResponse>
{
    private readonly IIdentityProvider _identityProvider;

    public GetRoleByIdHandler(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var roleResult = await _identityProvider.GetRoleByNameAsync(request.RoleName, cancellationToken);

        if (roleResult.IsFailure)
        {
            return Result.Failure<RoleResponse>(roleResult.Error);
        }

        var role = roleResult.Value;
        var roleResponse = new RoleResponse(0, role.Name, role.Description);

        return Result.Success(roleResponse);
    }
}
