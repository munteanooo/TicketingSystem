using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Contracts.Interfaces
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<Ticket?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Ticket>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);
        Task<IEnumerable<Ticket>> GetByAssignedTechnicianAsync(Guid technicianId, CancellationToken cancellationToken);
        Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken cancellationToken);
        Task<Ticket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken);
        Task AddMessageAsync(TicketMessage message, CancellationToken cancellationToken);
    }
}