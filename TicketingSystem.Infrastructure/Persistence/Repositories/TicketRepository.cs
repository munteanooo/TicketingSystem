using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(TicketingSystemDbContext context) : base(context) { }

        public async Task<IEnumerable<Ticket>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(t => t.ClientId == clientId).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByAssignedTechnicianAsync(Guid technicianId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(t => t.AssignedTechnicianId == technicianId).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(t => t.Status == status).ToListAsync(cancellationToken);
        }

        public async Task<Ticket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);
        }

        public async Task AddMessageAsync(TicketMessage message, CancellationToken cancellationToken)
        {
            // Adăugăm mesajul direct în DbSet-ul de mesaje pentru a evita încărcarea întregii liste de mesaje a ticheului
            await _context.Set<TicketMessage>().AddAsync(message, cancellationToken);
        }
    }
}