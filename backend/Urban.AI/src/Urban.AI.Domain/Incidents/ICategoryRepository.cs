namespace Urban.AI.Domain.Incidents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
#endregion

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<List<Category>> GetAllWithSubcategoriesAsync(CancellationToken cancellationToken);
}
