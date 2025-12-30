namespace ClientWeb.Services.Interfaces
{
    using ClientWeb.Models;

    public interface ITicketService
    {
        Task<bool> CreateTicketAsync(CreateTicketRequest request);
        Task<List<TicketDto>?> GetMyTicketsAsync();
        Task<TicketDto?> GetTicketByIdAsync(int id);
        Task<bool> UpdateTicketAsync(int id, UpdateTicketRequest request);
        Task<bool> DeleteTicketAsync(int id);
    }
}