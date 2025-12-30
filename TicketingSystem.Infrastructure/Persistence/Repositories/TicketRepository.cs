using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using Client.Application.Contracts.Persistence;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<List<Ticket>> GetByAgentIdAsync(Guid agentId)
        {
            return await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedToAgent)
                .Where(t => t.AssignedToAgentId == agentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedToAgent)
                .ToListAsync(cancellationToken);
        }
        public async Task<Ticket?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedToAgent)
                .Include(t => t.Messages)
                .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<Ticket>> GetByUserAsync(Guid clientId)
        {
            return await _context.Tickets
                .Where(t => t.ClientId == clientId)
                .ToListAsync();
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var ticket = await GetByIdAsync(id);
            if (ticket is null)
                return;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
