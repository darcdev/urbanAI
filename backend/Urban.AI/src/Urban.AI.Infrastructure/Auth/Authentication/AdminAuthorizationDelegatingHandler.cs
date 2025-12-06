namespace Urban.AI.Infrastructure.Auth.Authentication;

#region Usings
using Urban.AI.Infrastructure.Auth.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
#endregion

internal sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
    #region Constants
    private const string ClientIdParameterName = "client_id";
    private const string ClientSecretParameterName = "client_secret";
    private const string ScopeParameterName = "scope";
    private const string GrantTypeParameterName = "grant_type";
    private const string OpenIdScopeValue = "openid email";
    private const string ClientCredentialsGrantTypeValue = "client_credentials"; 
    #endregion

    private readonly KeycloakOptions _keycloakOptions;

    public AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keycloakOptions)
    {
        _keycloakOptions = keycloakOptions.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        AuthorizationToken authorizationToken = await GetAuthorizationToken(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            authorizationToken.AccessToken);

        var httpResponseMessage = await base.SendAsync(request, cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return httpResponseMessage;
    }

    private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
    {
        var authorizationRequestParameters = new KeyValuePair<string, string>[]
        {
            new(ClientIdParameterName, _keycloakOptions.AdminClientId),
            new(ClientSecretParameterName, _keycloakOptions.AdminClientSecret),
            new(ScopeParameterName, OpenIdScopeValue),
            new(GrantTypeParameterName, ClientCredentialsGrantTypeValue)
        };

        var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameters);

        using var authorizationRequest = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(_keycloakOptions.TokenUrl))
        {
            Content = authorizationRequestContent
        };

        var authorizationResponse = await base.SendAsync(authorizationRequest, cancellationToken);

        authorizationResponse.EnsureSuccessStatusCode();

        return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken) ??
               throw new ApplicationException();
    }
}
