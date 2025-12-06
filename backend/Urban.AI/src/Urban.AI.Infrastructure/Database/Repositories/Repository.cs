namespace Urban.AI.Infrastructure.Database.Repositories;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
#endregion

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    #region Constants
    private const string IsDeletedPropertyName = "IsDeleted";
    #endregion

    protected readonly ApplicationDbContext _dbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<T>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<T>()
            .Where(entity => ids.Contains(entity.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<T>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<T>()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<T>()
            .AnyAsync(predicate, cancellationToken);
    }

    public virtual void Add(T entity)
    {
        _dbContext.Add(entity);
    }

    public virtual void AddMany(IEnumerable<T> entities)
    {
        _dbContext.AddRange(entities);
    }

    public virtual void Update(T entity)
    {
        _dbContext.Update(entity);
    }

    public virtual bool Delete(T entity)
    {
        _dbContext.Remove(entity);
        return true;
    }

    public virtual bool SoftDelete(T entity)
    {
        var property = typeof(T).GetProperty(IsDeletedPropertyName);
        if (property is not null && property.PropertyType == typeof(bool))
        {
            property.SetValue(entity, true);
            _dbContext.Update(entity);
            return true;
        }
        return false;
    }
}