using System.Net.Http.Json;
using TicketingSystem.Application.Tickets.Queries.GetClientTickets; 

namespace TicketingSystem.Blazor.Services;

public class TicketService
{
    private readonly HttpClient _httpClient;

    public TicketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GetClientTicketsQueryResponseDto>> GetTickets()
    {
        try
        {
            var tickets = await _httpClient.GetFromJsonAsync<List<GetClientTicketsQueryResponseDto>>("api/tickets/my-tickets");
            return tickets ?? new List<GetClientTicketsQueryResponseDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la preluarea tichetelor: {ex.Message}");
            return new List<GetClientTicketsQueryResponseDto>();
        }
    }
}