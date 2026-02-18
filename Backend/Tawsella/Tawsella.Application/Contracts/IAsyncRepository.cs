using System.Threading;
using System.Threading.Tasks;

namespace Tawsella.Application.Contracts
{
    /// <summary>
    /// Base repository interface for common CRUD operations.
    /// All feature-specific repositories inherit from this.
    /// </summary>
    public interface IAsyncRepository<T> where T : class
    {
        /// <summary>
        /// Get entity by ID.
        /// </summary>
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add new entity and save changes.
        /// </summary>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update existing entity and save changes.
        /// </summary>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete entity and save changes.
        /// </summary>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
