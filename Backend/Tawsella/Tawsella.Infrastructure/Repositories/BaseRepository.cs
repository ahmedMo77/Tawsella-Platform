using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository implementation with common CRUD operations.
    /// All feature-specific repositories inherit from this.
    /// </summary>
    public class BaseRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
