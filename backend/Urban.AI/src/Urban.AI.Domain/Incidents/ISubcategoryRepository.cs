namespace Urban.AI.Domain.Incidents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
#endregion

public interface ISubcategoryRepository : IRepository<Subcategory>
{
    Task<List<Subcategory>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken);
}
