namespace Urban.AI.Infrastructure.Database.Repositories.Incident;

#region Usings
using Urban.AI.Domain.Incidents;
using Microsoft.EntityFrameworkCore;
#endregion

internal sealed class SubcategoryRepository : Repository<Subcategory>, ISubcategoryRepository
{
    public SubcategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Subcategory>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Subcategory>()
            .Where(s => s.CategoryId == categoryId)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }
}
