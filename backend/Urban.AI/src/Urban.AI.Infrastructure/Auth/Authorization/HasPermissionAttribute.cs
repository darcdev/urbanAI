namespace Urban.AI.Infrastructure.Auth.Authorization;

#region Usings
using Microsoft.AspNetCore.Authorization; 
#endregion

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(permission)
    {
    }
}
