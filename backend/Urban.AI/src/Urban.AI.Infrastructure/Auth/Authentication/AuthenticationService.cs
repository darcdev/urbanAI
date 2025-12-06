namespace Urban.AI.Infrastructure.Auth.Authentication;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Domain.Users;
using Urban.AI.Infrastructure.Auth.Authentication.Models;
using System.Net.Http.Json;
#endregion

internal sealed class AuthenticationService : IAuthenticationService
{
    #region Constants
    private const string PasswordCredentialType = "password";
    private const string UsersEndpoint = "users";
    private const string UsersSegmentName = "users/";
    private const bool IsTemporalPassword = false;
    #endregion

    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(user);

        userRepresentationModel.Credentials = new CredentialRepresentationModel[]
        {
            new()
            {
                Value = password,
                Temporary = IsTemporalPassword,
                Type = PasswordCredentialType
            }
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            UsersEndpoint,
            userRepresentationModel,
            cancellationToken);

        return ExtractIdentityIdFromLocationHeader(response);
    }

    private static string ExtractIdentityIdFromLocationHeader(
        HttpResponseMessage httpResponseMessage)
    {
        var locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header can't be null");
        }

        var userSegmentValueIndex = locationHeader.IndexOf(
            UsersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        var userIdentityId = locationHeader.Substring(userSegmentValueIndex + UsersSegmentName.Length);

        return userIdentityId;
    }
}
