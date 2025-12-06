namespace Urban.AI.Domain.Common.Abstractions;

using System.Linq.Expressions;

/// <summary>
/// Defines the general methods of a repository.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>  
    /// Retrieves an entity by its unique identifier.  
    /// </summary>  
    /// <param name="id">The unique identifier of the entity.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>The entity if found; otherwise, null.</returns>  
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>  
    /// Retrieves multiple entities by their unique identifiers.  
    /// </summary>  
    /// <param name="ids">A collection of unique identifiers.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>A collection of entities matching the provided identifiers.</returns>  
    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>  
    /// Retrieves all entities of the specified type.  
    /// </summary>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>A collection of all entities.</returns>  
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>  
    /// Finds entities that match the specified predicate.  
    /// </summary>  
    /// <param name="predicate">A lambda expression to filter the entities.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>A collection of entities that satisfy the predicate.</returns>  
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>  
    /// Checks if any entity matches the specified predicate.  
    /// </summary>  
    /// <param name="predicate">A lambda expression to filter the entities.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>True if any entity matches the predicate; otherwise, false.</returns>  
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>  
    /// Adds a new entity to the repository.  
    /// </summary>  
    /// <param name="entity">The entity to add.</param>  
    void Add(T entity);

    /// <summary>
    /// Adds multiple entities to the repository.
    /// </summary>
    void AddMany(IEnumerable<T> entities);

    /// <summary>  
    /// Deletes an entity from the database.  
    /// </summary>  
    /// <param name="entity">The entity to delete.</param>  
    /// <returns>True if the entity was successfully deleted; otherwise, false.</returns>  
    bool Delete(T entity);

    /// <summary>  
    /// Marks an entity as deleted without removing it from the database.  
    /// </summary>  
    /// <param name="entity">The entity to soft delete.</param>  
    /// <returns>True if the entity was successfully marked as deleted; otherwise, false.</returns>  
    bool SoftDelete(T entity);
}
