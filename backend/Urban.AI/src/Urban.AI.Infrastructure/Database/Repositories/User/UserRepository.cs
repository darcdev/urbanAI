namespace Urban.AI.Infrastructure.Database.Repositories.User;

#region Usings
using Urban.AI.Domain.Users;
using Microsoft.EntityFrameworkCore;
#endregion

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override void Add(User user)
    {
        foreach (var role in user.Roles)
        {
            _dbContext.Attach(role);
        }

        _dbContext.Add(user);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Roles)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.UserDetails)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailWithDetailsAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.UserDetails)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
