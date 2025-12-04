using TicketingSystem.ClientApi.Models;

namespace Client.Web.Services
{
    public class TicketService : ITicketService
    {
        private readonly HttpClient _httpClient;

        public TicketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TicketDto>> GetTicketsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<TicketDto>>("api/tickets");
                return response ?? new List<TicketDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tickets: {ex.Message}");
                return new List<TicketDto>();
            }
        }

        public async Task<TicketDetailDto?> GetTicketAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TicketDetailDto>($"api/tickets/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading ticket {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<TicketDetailDto> CreateTicketAsync(CreateTicketRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/tickets", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TicketDetailDto>()
                ?? throw new Exception("Failed to create ticket");
        }

        // Implementează restul metodelor pe măsură ce le adaugi în API
    }
}