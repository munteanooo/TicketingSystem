using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(TicketingSystemDbContext context) : base(context) { }

        public override async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.Messages).ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<Ticket?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Ticket>()
                .IgnoreQueryFilters()
                .Include(t => t.Client)
                .Include(t => t.AssignedTechnician)
                .Include(t => t.Messages).ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(t => t.Status == status).ToListAsync(cancellationToken);
        }

        public async Task<Ticket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(t => t.ClientId == clientId).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetByAssignedTechnicianAsync(Guid technicianId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(t => t.AssignedTechnicianId == technicianId).ToListAsync(cancellationToken);
        }

        public async Task AddMessageAsync(TicketMessage message, CancellationToken cancellationToken)
        {
            await _context.Set<TicketMessage>().AddAsync(message, cancellationToken);
        }
    }
}