namespace Urban.AI.Infrastructure.Auth.Authentication;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.UserManagement.Users.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Infrastructure.Auth.Authentication.Models;
using Urban.AI.Infrastructure.Auth.Resources;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
#endregion

internal sealed class KeycloakService(HttpClient httpClient) : IIdentityProvider
{
    #region Constants
    private const string UsersEndpoint = "users";
    private const string RolesEndpoint = "clients/enterprisetemplate-auth-client/roles";
    private const string RoleMappingsSegment = "role-mappings/realm";
    #endregion

    private readonly HttpClient _httpClient = httpClient;

    public async Task<Result<IEnumerable<UserResponse>>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(UsersEndpoint, cancellationToken);
            if (!response.IsSuccessStatusCode) return Result.Failure<IEnumerable<UserResponse>>(AuthErrors.FailedToRetrieveUsers);

            var dataJson = response.Content.ReadAsStringAsync(cancellationToken);
            var users = await response.Content.ReadFromJsonAsync<List<UserRepresentationModel>>(cancellationToken);
            if (users is null) return Result.Failure<IEnumerable<UserResponse>>(AuthErrors.FailedToRetrieveUsers);

            var userResponses = users.Select(user => user.ToUserResponse());

            return Result.Success(userResponses);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<IEnumerable<UserResponse>>(AuthErrors.FailedToRetrieveUsers);
        }
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{UsersEndpoint}/{identityId}", cancellationToken);
            if (response.StatusCode is HttpStatusCode.NotFound) return Result.Failure<UserResponse>(AuthErrors.UserNotFoundInKeycloak);
            if (!response.IsSuccessStatusCode) return Result.Failure<UserResponse>(AuthErrors.FailedToRetrieveUser);

            var user = await response.Content.ReadFromJsonAsync<UserRepresentationModel>(cancellationToken);
            if (user is null) return Result.Failure<UserResponse>(AuthErrors.FailedToRetrieveUser);

            var userResponse = user.ToUserResponse();

            return Result.Success(userResponse);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<UserResponse>(AuthErrors.FailedToRetrieveUser);
        }
    }

    public async Task<Result> UpdateUserAsync(
        string identityId,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userUpdate = new
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = email
            };

            var response = await _httpClient.PutAsJsonAsync($"{UsersEndpoint}/{identityId}", userUpdate, cancellationToken);
            if (response.StatusCode is HttpStatusCode.NotFound) return Result.Failure(AuthErrors.UserNotFoundInKeycloak);
            if (!response.IsSuccessStatusCode) return Result.Failure(AuthErrors.FailedToUpdateUser);

            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return Result.Failure(AuthErrors.FailedToUpdateUser);
        }
    }

    public async Task<Result> DeleteUserAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{UsersEndpoint}/{identityId}", cancellationToken);

            if (response.StatusCode is HttpStatusCode.NotFound) return Result.Failure(AuthErrors.UserNotFoundInKeycloak);
            if (!response.IsSuccessStatusCode) return Result.Failure(AuthErrors.FailedToDeleteUser);

            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return Result.Failure(AuthErrors.FailedToDeleteUser);
        }
    }

    public async Task<Result> AssignRolesToUserAsync(
        string identityId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rolesToAssign = new List<RoleRepresentationModel>();

            foreach (var roleName in roleNames)
            {
                var roleResult = await GetRoleByNameAsync(roleName, cancellationToken);
                if (roleResult.IsFailure) return Result.Failure(AuthErrors.RoleNotFound(roleName));

                var role = roleResult.Value;
                rolesToAssign.Add(new RoleRepresentationModel { Id = role.Id, Name = role.Name });
            }

            var response = await _httpClient.PostAsJsonAsync(
                $"{UsersEndpoint}/{identityId}/{RoleMappingsSegment}",
                rolesToAssign,
                cancellationToken);
            if (!response.IsSuccessStatusCode) return Result.Failure(AuthErrors.FailedToAssignRoles);

            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return Result.Failure(AuthErrors.FailedToAssignRoles);
        }
    }

    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{UsersEndpoint}/{identityId}/{RoleMappingsSegment}", cancellationToken);
            if (!response.IsSuccessStatusCode) return Result.Failure<IEnumerable<string>>(AuthErrors.FailedToRetrieveUserRoles);

            var roles = await response.Content.ReadFromJsonAsync<List<RoleRepresentationModel>>(cancellationToken);
            if (roles is null) return Result.Failure<IEnumerable<string>>(AuthErrors.FailedToRetrieveUserRoles);

            return Result.Success(roles.Select(r => r.Name));
        }
        catch (HttpRequestException)
        {
            return Result.Failure<IEnumerable<string>>(AuthErrors.FailedToRetrieveUserRoles);
        }
    }

    public async Task<Result<IEnumerable<RoleResponse>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(RolesEndpoint, cancellationToken);
            if (!response.IsSuccessStatusCode) return Result.Failure<IEnumerable<RoleResponse>>(AuthErrors.FailedToRetrieveRoles);

            var roles = await response.Content.ReadFromJsonAsync<List<RoleRepresentationModel>>(cancellationToken);
            if (roles is null) return Result.Failure<IEnumerable<RoleResponse>>(AuthErrors.FailedToRetrieveRoles);

            var roleResponses = roles.Select(r => new RoleResponse(
                r.Id,
                r.Name,
                r.Description ?? string.Empty,
                r.Composite,
                r.ClientRole
            ));

            return Result.Success(roleResponses);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<IEnumerable<RoleResponse>>(AuthErrors.FailedToRetrieveRoles);
        }
    }

    public async Task<Result<RoleResponse>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{RolesEndpoint}/{roleName}", cancellationToken);
            if (response.StatusCode is HttpStatusCode.NotFound) return Result.Failure<RoleResponse>(AuthErrors.RoleNotFound(roleName));
            if (!response.IsSuccessStatusCode) return Result.Failure<RoleResponse>(AuthErrors.FailedToRetrieveRole);

            var role = await response.Content.ReadFromJsonAsync<RoleRepresentationModel>(cancellationToken);

            if (role is null) return Result.Failure<RoleResponse>(AuthErrors.FailedToRetrieveRole);

            var roleResponse = new RoleResponse(
                role.Id,
                role.Name,
                role.Description ?? string.Empty,
                role.Composite,
                role.ClientRole
            );

            return Result.Success(roleResponse);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<RoleResponse>(AuthErrors.FailedToRetrieveRole);
        }
    }

    public async Task<Result> CreateRoleAsync(string roleName, string description, CancellationToken cancellationToken = default)
    {
        try
        {
            var newRole = new RoleRepresentationModel
            {
                Name = roleName,
                Description = description
            };

            var response = await _httpClient.PostAsJsonAsync(RolesEndpoint, newRole, cancellationToken);
            if (response.StatusCode is HttpStatusCode.Conflict) return Result.Failure(AuthErrors.RoleAlreadyExists(roleName));
            if (!response.IsSuccessStatusCode) return Result.Failure(AuthErrors.FailedToCreateRole);

            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return Result.Failure(AuthErrors.FailedToCreateRole);
        }
    }

    public async Task<Result> UpdateRoleAsync(string roleName, string newDescription, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleUpdate = new RoleRepresentationModel
            {
                Name = roleName,
                Description = newDescription
            };

            var response = await _httpClient.PutAsJsonAsync($"{RolesEndpoint}/{roleName}", roleUpdate, cancellationToken);
            if (response.StatusCode is HttpStatusCode.NotFound) return Result.Failure(AuthErrors.RoleNotFound(roleName));
            if (!response.IsSuccessStatusCode) return Result.Failure(AuthErrors.FailedToUpdateRole);

            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return Result.Failure(AuthErrors.FailedToUpdateRole);
        }
    }

    public async Task<Result> DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{RolesEndpoint}/{roleName}", cancellationToken);
            if (response.StatusCode is HttpStatusCode.NotFound) return Result.Failure(AuthErrors.RoleNotFound(roleName));
            if (!response.IsSuccessStatusCode) return Result.Failure(AuthErrors.FailedToDeleteRole);

            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return Result.Failure(AuthErrors.FailedToDeleteRole);
        }
    }
}
