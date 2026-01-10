using TicketingSystem.Domain.Entities;

namespace Client.Application.Contracts.Messaging
{
    public interface ITicketMessageRepository
    {
        Task<TicketMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<TicketMessage>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);
        Task AddAsync(TicketMessage message, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
