using System.Text.Json;
using ClientUI.Models;
using ClientUI.Services.Interfaces;

public class TicketService : ITicketService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private const string API_BASE_URL = "/api/Tickets";
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TicketService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<bool> CreateTicketAsync(CreateTicketRequest request)
    {
        try
        {
            await _authService.SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/create", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateTicket error: {ex.Message}");
            return false;
        }
    }

    public async Task<List<TicketDto>?> GetMyTicketsAsync()
    {
        try
        {
            await _authService.SetAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"{API_BASE_URL}/my-tickets");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            var wrapper = JsonSerializer.Deserialize<MyTicketsResponseWrapper>(json, _jsonOptions);
            return wrapper?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetMyTickets error: {ex.Message}");
            return null;
        }
    }

    public async Task<TicketDto?> GetTicketByIdAsync(Guid id)
    {
        try
        {
            await _authService.SetAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"{API_BASE_URL}/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TicketDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetTicketById error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateTicketAsync(Guid id, UpdateTicketRequest request)
    {
        try
        {
            await _authService.SetAuthorizationHeaderAsync();

            var response = await _httpClient.PutAsJsonAsync($"{API_BASE_URL}/{id}", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UpdateTicket error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CloseTicketAsync(Guid id)
    {
        try
        {
            await _authService.SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsync($"{API_BASE_URL}/{id}/close", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloseTicket error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ReopenTicketAsync(Guid id, string reason)
    {
        try
        {
            await _authService.SetAuthorizationHeaderAsync();
            var body = new { Reason = reason };
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/{id}/reopen", body);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ReopenTicket error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AddMessageAsync(Guid ticketId, string content)
    {
        try
        {
            await _authService.SetAuthorizationHeaderAsync();
            var body = new { Content = content };
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/{ticketId}/messages", body);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AddMessage error: {ex.Message}");
            return false;
        }
    }

    private class MyTicketsResponseWrapper
    {
        public bool Success { get; set; }
        public List<TicketDto>? Data { get; set; }
        public int Count { get; set; }
    }
}