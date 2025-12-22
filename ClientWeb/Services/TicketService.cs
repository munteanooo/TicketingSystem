using System.Net.Http.Json;
using System.Text.Json;
using ClientWeb.Models;
using ClientWeb.Services.Interfaces;

namespace ClientWeb.Services
{
    public class TicketService : ITicketService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private const string API_BASE_URL = "/api/ticket";
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

                var response = await _httpClient.PostAsJsonAsync(
                    $"{API_BASE_URL}/create",
                    request
                );

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

                var response = await _httpClient.GetAsync($"{API_BASE_URL}/my");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TicketDto>>(json, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMyTickets error: {ex.Message}");
                return null;
            }
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id)
        {
            try
            {
                await _authService.SetAuthorizationHeaderAsync();

                var response = await _httpClient.GetAsync($"{API_BASE_URL}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TicketDto>(json, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTicketById error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateTicketAsync(int id, UpdateTicketRequest request)
        {
            try
            {
                await _authService.SetAuthorizationHeaderAsync();

                var response = await _httpClient.PutAsJsonAsync(
                    $"{API_BASE_URL}/{id}",
                    request
                );

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTicket error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            try
            {
                await _authService.SetAuthorizationHeaderAsync();

                var response = await _httpClient.DeleteAsync($"{API_BASE_URL}/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteTicket error: {ex.Message}");
                return false;
            }
        }
    }
}