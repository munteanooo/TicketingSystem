using TicketingSystem.Domain.Entities;

namespace Client.Application.Contracts.Persistence
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<List<Ticket>> GetAllAsync();
        Task<List<Ticket>> GetByUserAsync(Guid clientId);
        Task<List<Ticket>> GetByAgentIdAsync(Guid agentId);
        Task<Ticket?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(Guid id);
    }
}
