namespace Urban.AI.Domain.Users;

using Urban.AI.Domain.Common.Abstractions;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllWithRolesAsync(CancellationToken cancellationToken);
    Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailWithDetailsAsync(string email, CancellationToken cancellationToken);
}