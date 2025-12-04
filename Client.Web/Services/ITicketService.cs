using TicketingSystem.ClientApi.Models;

namespace Client.Web.Services
{
    public interface ITicketService
    {
        Task<List<TicketDto>> GetTicketsAsync();
        Task<TicketDetailDto?> GetTicketAsync(int id);
        Task<TicketDetailDto> CreateTicketAsync(CreateTicketRequest request);

        // Comentează temporar dacă endpoint-urile nu există încă în API
        // Task<TicketDetailDto> UpdateTicketAsync(int id, UpdateTicketRequest request);
        // Task<bool> DeleteTicketAsync(int id);
        // Task<MessageDto> AddMessageAsync(int ticketId, AddMessageRequest request);
        // Task<StatsDto> GetStatsAsync();
        // Task<List<TicketDto>> GetMyAssignedTicketsAsync();
        // Task<List<TicketDto>> GetOpenTicketsAsync();
    }
}