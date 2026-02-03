using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Abstract base repository implementing generic CRUD operations
    /// All repositories should inherit from this class
    /// </summary>
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly TicketingSystemDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initialize the repository with database context
        /// </summary>
        protected BaseRepository(TicketingSystemDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Get a single entity by ID
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            return await _dbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Find entities matching a predicate
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get the first entity matching a predicate or null
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Check if an entity exists matching a predicate
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Count entities, optionally with a predicate
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            return predicate == null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Add a new entity
        /// </summary>
        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Add multiple new entities
        /// </summary>
        public virtual async Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            if (!entityList.Any())
                throw new ArgumentException("Entity collection is empty", nameof(entities));

            await _dbSet.AddRangeAsync(entityList, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Update an existing entity
        /// </summary>
        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Update multiple entities
        /// </summary>
        public virtual async Task UpdateRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            if (!entityList.Any())
                throw new ArgumentException("Entity collection is empty", nameof(entities));

            _dbSet.UpdateRange(entityList);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Delete an entity
        /// </summary>
        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Delete an entity by ID
        /// </summary>
        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Delete multiple entities
        /// </summary>
        public virtual async Task DeleteRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            if (!entityList.Any())
                throw new ArgumentException("Entity collection is empty", nameof(entities));

            _dbSet.RemoveRange(entityList);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Save all changes to the database
        /// </summary>
        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database update failed. See inner exception for details.", ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new InvalidOperationException("Database operation was cancelled.", ex);
            }
        }
    }
}