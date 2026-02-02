using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Application.Contracts.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Ticket?> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetByAssignedTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
        Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}