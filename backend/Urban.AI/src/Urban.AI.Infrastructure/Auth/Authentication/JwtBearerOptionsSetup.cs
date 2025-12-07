namespace Urban.AI.Infrastructure.Auth.Authentication;

#region Usings
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
#endregion

internal sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    #region Constants
    private const string RealmAccessClaim = "realm_access";
    private const string RolesClaim = "roles";
    #endregion

    private readonly AuthenticationOptions _authenticationOptions;

    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    {
        _authenticationOptions = authenticationOptions.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = _authenticationOptions.Audience;
        options.MetadataAddress = _authenticationOptions.MetadataUrl;
        options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
        options.TokenValidationParameters.ValidIssuer = _authenticationOptions.Issuer;

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                MapKeycloakRolesToClaimRoles(context);
                return Task.CompletedTask;
            }
        };
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    private static void MapKeycloakRolesToClaimRoles(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity)
        {
            return;
        }

        var realmAccessClaim = context.Principal.FindFirst(RealmAccessClaim);
        if (realmAccessClaim?.Value is null)
        {
            return;
        }

        using var realmAccess = JsonDocument.Parse(realmAccessClaim.Value);
        if (!realmAccess.RootElement.TryGetProperty(RolesClaim, out var rolesElement))
        {
            return;
        }

        var roles = rolesElement.EnumerateArray()
            .Where(role => role.GetString() is not null)
            .Select(role => new Claim(ClaimTypes.Role, role.GetString()!));

        identity.AddClaims(roles);
    }
}
