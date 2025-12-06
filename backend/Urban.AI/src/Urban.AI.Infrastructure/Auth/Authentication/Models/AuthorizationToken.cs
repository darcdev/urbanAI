namespace Urban.AI.Infrastructure.Auth.Authentication.Models;

#region Usings
using System.Text.Json.Serialization;
#endregion

internal sealed class AuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
}
