namespace ClientUI.Services.Interfaces
{
    using ClientUI.Models;

    public interface ITicketService
    {
        Task<bool> CreateTicketAsync(CreateTicketRequest request);
        Task<List<TicketDto>?> GetMyTicketsAsync();
        Task<TicketDto?> GetTicketByIdAsync(Guid id);
        Task<bool> UpdateTicketAsync(Guid id, UpdateTicketRequest request);
        Task<bool> CloseTicketAsync(Guid id);
        Task<bool> ReopenTicketAsync(Guid id, string reason);
        Task<bool> AddMessageAsync(Guid ticketId, string content);
    }
}