namespace Urban.AI.Domain.Common.Abstractions;

/// <summary>
/// Unit of work is a design pattern used to ensure data integrity when
/// multiple database operations are performed in a single transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}