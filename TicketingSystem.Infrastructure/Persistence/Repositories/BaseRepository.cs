using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.Contracts.Interfaces;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly TicketingSystemDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(TicketingSystemDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.ToListAsync(cancellationToken);

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => predicate == null ? await _dbSet.CountAsync(cancellationToken) : await _dbSet.CountAsync(predicate, cancellationToken);

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
            => await Task.Run(() => _dbSet.Update(entity), cancellationToken);

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            => await Task.Run(() => _dbSet.UpdateRange(entities), cancellationToken);

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
            => await Task.Run(() => _dbSet.Remove(entity), cancellationToken);

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null) _dbSet.Remove(entity);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            => await Task.Run(() => _dbSet.RemoveRange(entities), cancellationToken);

        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}