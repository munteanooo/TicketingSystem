using System.Net.Http.Json;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Blazor.Services;

public class TicketService
{
    private readonly HttpClient _httpClient;

    public TicketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Ticket>> GetAllTickets()
    {
        try
        {
            var tickets = await _httpClient.GetFromJsonAsync<List<Ticket>>("api/tickets");
            return tickets ?? new List<Ticket>();
        }
        catch (Exception)
        {
            return new List<Ticket>();
        }
    }

    public async Task<Ticket?> GetTicketById(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<Ticket>($"api/tickets/{id}");
    }
}