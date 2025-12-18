using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status);
        Task<IEnumerable<Ticket>> GetByAssigneeAsync(int userId);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
    }
}
