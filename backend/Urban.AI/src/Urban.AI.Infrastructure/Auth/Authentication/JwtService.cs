namespace Urban.AI.Infrastructure.Auth.Authentication;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Infrastructure.Auth.Authentication.Models;
using Urban.AI.Infrastructure.Auth.Resources;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
#endregion

internal sealed class JwtService : IJwtService
{
    #region Constants
    private const string ConcatenateRolesDelimiter = ", ";
    private const int TokenExpirationTimeInHours = 5;
    private const string AuthEndpoint = "";
    private const string ClientIdParameterName = "client_id";
    private const string ClientSecretParameterName = "client_secret";
    private const string ScopeParameterName = "scope";
    private const string GrantTypeParameterName = "grant_type";
    private const string OpenIdScopeValue = "openid email";
    private const string PasswordGrantTypeValue = "password";
    private const string UsernameParameterName = "username";
    private const string PasswordParameterName = "password";
    #endregion

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;

    public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    public async Task<Result<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new(ClientIdParameterName, _keycloakOptions.AuthClientId),
                new(ClientSecretParameterName, _keycloakOptions.AuthClientSecret),
                new(ScopeParameterName, OpenIdScopeValue),
                new(GrantTypeParameterName, PasswordGrantTypeValue),
                new(UsernameParameterName, email),
                new(PasswordParameterName, password)
            };

            using var authorizationRequestContent = new FormUrlEncodedContent(authRequestParameters);

            var response = await _httpClient.PostAsync(
                AuthEndpoint,
                authorizationRequestContent,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var authorizationToken = await response
                .Content
                .ReadFromJsonAsync<AuthorizationToken>(cancellationToken);

            if (authorizationToken is null)
            {
                return Result.Failure<string>(AuthErrors.AuthenticationFailed);
            }

            return authorizationToken.AccessToken;
        }
        catch (HttpRequestException)
        {
            return Result.Failure<string>(AuthErrors.AuthenticationFailed);
        }
    }
}
