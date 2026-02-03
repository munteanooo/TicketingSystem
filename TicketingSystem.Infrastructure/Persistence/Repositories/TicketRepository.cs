using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository for Ticket entity providing ticket-specific queries
    /// </summary>
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(TicketingSystemDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get ticket by ticket number with full navigation properties loaded
        /// </summary>
        public async Task<Ticket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ticketNumber))
                throw new ArgumentException("Ticket number cannot be empty", nameof(ticketNumber));

            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);
        }

        /// <summary>
        /// Get all tickets created by a specific client
        /// Ordered by creation date (newest first)
        /// </summary>
        public async Task<IEnumerable<Ticket>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            if (clientId == Guid.Empty)
                throw new ArgumentException("Client ID cannot be empty", nameof(clientId));

            return await _dbSet
                .Where(t => t.ClientId == clientId)
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get all tickets assigned to a specific technician
        /// Ordered by creation date (newest first)
        /// </summary>
        public async Task<IEnumerable<Ticket>> GetByAssignedTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default)
        {
            if (technicianId == Guid.Empty)
                throw new ArgumentException("Technician ID cannot be empty", nameof(technicianId));

            return await _dbSet
                .Where(t => t.AssignedTechnicianId == technicianId)
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get all tickets with a specific status
        /// Ordered by creation date (newest first)
        /// </summary>
        public async Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get a single ticket by ID with all related entities loaded
        /// </summary>
        public override async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        /// <summary>
        /// Get all tickets with related entities loaded
        /// Ordered by creation date (newest first)
        /// </summary>
        public override async Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Add a new ticket and automatically save
        /// </summary>
        public override async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            await _dbSet.AddAsync(ticket, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Update a ticket and set the UpdatedAt timestamp
        /// </summary>
        public override async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            ticket.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(ticket);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Delete a ticket by ID
        /// </summary>
        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            var ticket = await GetByIdAsync(id, cancellationToken);
            if (ticket != null)
            {
                _dbSet.Remove(ticket);
                await SaveChangesAsync(cancellationToken);
            }
        }
    }
}