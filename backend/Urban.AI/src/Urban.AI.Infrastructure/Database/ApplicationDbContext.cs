namespace Urban.AI.Infrastructure.Database;

#region Usings
using Microsoft.EntityFrameworkCore;
using Urban.AI.Application.Common.Exceptions;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
using Urban.AI.Domain.Users;
#endregion

public sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    #region DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Township> Townships { get; set; }
    #endregion

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
