namespace Urban.AI.Application.Common.Abstractions.Authentication;

#region Usings
using Urban.AI.Application.UserManagement.Users.Dtos;
using Urban.AI.Domain.Common.Abstractions;
#endregion

public interface IIdentityProvider
{
    Task<Result<IEnumerable<UserResponse>>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetUserByIdAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result> UpdateUserAsync(string identityId, string firstName, string lastName, string email, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result> AssignRolesToUserAsync(string identityId, IEnumerable<string> roleNames, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<string>>> GetUserRolesAsync(string identityId, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<RoleResponse>>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<Result<RoleResponse>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<Result> CreateRoleAsync(string roleName, string description, CancellationToken cancellationToken = default);
    Task<Result> UpdateRoleAsync(string roleName, string newDescription, CancellationToken cancellationToken = default);
    Task<Result> DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);
}
