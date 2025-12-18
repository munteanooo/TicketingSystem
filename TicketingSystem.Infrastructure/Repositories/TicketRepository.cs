using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Interfaces;
using TicketingSystem.Infrastructure.Data;

namespace TicketingSystem.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status)
        {
            return await _context.Tickets
                .Where(t => t.Status == status)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByAssigneeAsync(int userId)
        {
            return await _context.Tickets
                .Where(t => t.AssignedToId == userId)
                .Include(t => t.CreatedBy)
                .Include(t => t.Messages)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
                _context.Tickets.Remove(ticket);
        }
    }
}