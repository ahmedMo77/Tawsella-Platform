using System.Linq;

namespace Tawsella.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Queryable set for complex queries (Include, Where, ProjectTo, etc.).
        /// </summary>
        IQueryable<T> Query { get; }

        System.Threading.Tasks.Task<T> AddAsync(T entity, System.Threading.CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task AddRangeAsync(System.Collections.Generic.IEnumerable<T> entities, System.Threading.CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<T?> GetByIdAsync(string id, System.Threading.CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<T>> GetAllAsync(System.Threading.CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
    }
}
