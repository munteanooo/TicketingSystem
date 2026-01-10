using Client.Application.Contracts.Messaging;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Persistence.Repositories
{
    public class TicketMessageRepository : ITicketMessageRepository
    {
        private readonly AppDbContext _context;

        public TicketMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TicketMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.TicketMessages
                .FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<TicketMessage>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            return await _context.TicketMessages
                .Where(m => m.TicketId == ticketId)
                .Include(m => m.Author) // optional, dacă vrei să incluzi autorul
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(TicketMessage message, CancellationToken cancellationToken = default)
        {
            await _context.TicketMessages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var message = await GetByIdAsync(id, cancellationToken);
            if (message != null)
            {
                _context.TicketMessages.Remove(message);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
