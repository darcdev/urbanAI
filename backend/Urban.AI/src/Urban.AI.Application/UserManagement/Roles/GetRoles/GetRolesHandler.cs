namespace Urban.AI.Application.UserManagement.Roles.GetRoles;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Roles.Dtos;
using Urban.AI.Domain.Common.Abstractions;
#endregion

internal sealed class GetRolesHandler : IQueryHandler<GetRolesQuery, IEnumerable<RoleResponse>>
{
    private readonly IIdentityProvider _identityProvider;

    public GetRolesHandler(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<Result<IEnumerable<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var rolesResult = await _identityProvider.GetRolesAsync(cancellationToken);

        if (rolesResult.IsFailure)
        {
            return Result.Failure<IEnumerable<RoleResponse>>(rolesResult.Error);
        }

        var roleResponses = rolesResult.Value.Select(r => new RoleResponse(
            0,
            r.Name,
            r.Description
        ));

        return Result.Success(roleResponses);
    }
}
