using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(TicketingSystemDbContext context) : base(context)
        {
        }

        public async Task AddMessageAsync(TicketMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // Adăugăm mesajul direct în DbSet-ul de mesaje prin contextul partajat
            await _context.Set<TicketMessage>().AddAsync(message, cancellationToken);
        }

        public async Task<Ticket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByAssignedTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.AssignedTechnicianId == technicianId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public override async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Messages)
                    .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public override async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(ticket);
            await SaveChangesAsync(cancellationToken);
        }

        // ... restul metodelor (GetAllAsync, AddAsync, DeleteAsync) rămân neschimbate
    }
}