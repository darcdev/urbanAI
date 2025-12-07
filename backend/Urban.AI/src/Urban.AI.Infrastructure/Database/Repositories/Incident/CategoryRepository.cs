namespace Urban.AI.Infrastructure.Database.Repositories.Incident;

#region Usings
using Urban.AI.Domain.Incidents;
using Microsoft.EntityFrameworkCore;
#endregion

internal sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Category>()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<List<Category>> GetAllWithSubcategoriesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Category>()
            .Include(c => c.Subcategories)
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .ToListAsync(cancellationToken);
    }
}
