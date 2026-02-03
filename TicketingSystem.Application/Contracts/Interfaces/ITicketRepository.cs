using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Contracts.Interfaces
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<Ticket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetByAssignedTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken cancellationToken = default);

        Task AddMessageAsync(TicketMessage message, CancellationToken cancellationToken = default);
    }
}