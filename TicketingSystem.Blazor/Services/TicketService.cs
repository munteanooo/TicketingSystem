using System.Net.Http.Json;
using TicketingSystem.Blazor.Services.Interfaces;
using TicketingSystem.Application.Tickets.Commands.CreateTicket;
using TicketingSystem.Application.Tickets.Commands.AddMessage;
using TicketingSystem.Application.Tickets.Queries.GetClientTickets;
using TicketingSystem.Application.Tickets.Queries.GetTechTickets;
using TicketingSystem.Blazor.Models.DTOs;

namespace TicketingSystem.Blazor.Services;

public class TicketService : ITicketService
{
    private readonly HttpClient _httpClient;

    public TicketService(HttpClient httpClient) => _httpClient = httpClient;

    // 1. Obține tichetele clientului logat
    public async Task<List<GetClientTicketsQueryResponseDto>> GetTickets()
    {
        try
        {
            var resp = await _httpClient.GetAsync("api/tickets/my-tickets");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<List<GetClientTicketsQueryResponseDto>>() ?? new();
        }
        catch
        {
            return new List<GetClientTicketsQueryResponseDto>();
        }
    }

    // 2. Obține un singur tichet cu DETALII (și mesaje) - Aceasta lipsea probabil!
    public async Task<TicketDto?> GetTicketById(Guid id)
    {
        try
        {
            var resp = await _httpClient.GetAsync($"api/tickets/{id}");
            if (resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadFromJsonAsync<TicketDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    // 3. Obține tichetele nealocate (pentru TechSupport)
    public async Task<List<GetTechTicketsQueryResponseDto>> GetUnassignedTickets()
    {
        var resp = await _httpClient.GetAsync("api/tickets/unassigned");
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<GetTechTicketsQueryResponseDto>>() ?? new();
    }

    // 4. Obține tichetele deja preluate de tehnicianul curent
    public async Task<List<GetTechTicketsQueryResponseDto>> GetMyAssignedTickets()
    {
        var resp = await _httpClient.GetAsync("api/tickets/my-assigned");
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<GetTechTicketsQueryResponseDto>>() ?? new();
    }

    // 5. Creare tichet nou
    public async Task<CreateTicketCommandResponseDto?> CreateTicket(CreateTicketCommandDto ticketDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tickets", ticketDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CreateTicketCommandResponseDto>();
    }

    // 6. Actualizare status (Ex: InProgress, Resolved)
    public async Task<bool> UpdateTicketStatus(Guid ticketId, string status)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/tickets/{ticketId}/status", new { Status = status });
        return response.IsSuccessStatusCode;
    }

    // 7. Preluare tichet de către TechSupport
    public async Task<bool> AssignTicket(Guid ticketId)
    {
        // Unele API-uri cer un body gol {} chiar și la Put
        var response = await _httpClient.PutAsJsonAsync($"api/tickets/{ticketId}/assign", new { });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CloseTicket(Guid ticketId)
    {
        var dto = new
        {
            TicketId = ticketId,
            ResolutionNote = "Închis din interfața de suport"
        };

        var response = await _httpClient.PutAsJsonAsync($"api/tickets/{ticketId}/close", dto);
        return response.IsSuccessStatusCode;
    }

    // 9. Adăugare mesaj în conversație
    public async Task<AddMessageCommandResponseDto?> AddMessage(Guid ticketId, string content)
    {
        var dto = new AddMessageCommandDto
        {
            TicketId = ticketId,
            Content = content
        };

        var response = await _httpClient.PostAsJsonAsync($"api/tickets/{ticketId}/messages", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AddMessageCommandResponseDto>();
        }
        return null;
    }
}