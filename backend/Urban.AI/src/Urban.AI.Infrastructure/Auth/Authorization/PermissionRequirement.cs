namespace Urban.AI.Infrastructure.Auth.Authorization;

#region Usings
using Microsoft.AspNetCore.Authorization; 
#endregion

internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}
