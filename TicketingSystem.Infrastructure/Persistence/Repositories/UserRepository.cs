using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for User entity providing user-specific queries
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(TicketingSystemDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get a user by email address
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            return await _dbSet
                .Include(u => u.CreatedTickets)
                .Include(u => u.AssignedTickets)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        /// <summary>
        /// Get a user by ID with all related entities
        /// </summary>
        public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            return await _dbSet
                .Include(u => u.CreatedTickets)
                .Include(u => u.AssignedTickets)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <summary>
        /// Get all active users with their related tickets
        /// Ordered alphabetically by full name
        /// </summary>
        public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => u.IsActive)
                .Include(u => u.CreatedTickets)
                .Include(u => u.AssignedTickets)
                .OrderBy(u => u.FullName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get all active technicians with their assigned tickets
        /// Ordered alphabetically by full name
        /// </summary>
        public async Task<IEnumerable<User>> GetAllTechniciansAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => u.IsActive && u.Role == "Technician")
                .Include(u => u.AssignedTickets)
                .OrderBy(u => u.FullName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Add a new user and save to database
        /// </summary>
        public override async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("User email is required", nameof(user));

            // Check if email already exists
            var existingUser = await _dbSet.FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email '{user.Email}' already exists");

            await _dbSet.AddAsync(user, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Update a user and set the UpdatedAt timestamp
        /// </summary>
        public override async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(user);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Soft delete a user by setting IsActive to false
        /// </summary>
        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            var user = await GetByIdAsync(id, cancellationToken);
            if (user != null)
            {
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(user, cancellationToken);
            }
        }

        /// <summary>
        /// Check if a user exists by ID
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            return await _dbSet.AnyAsync(u => u.Id == id, cancellationToken);
        }
    }
}