namespace ClientWeb.Services
{
    public interface ITicketService
    {
        Task<List<TicketCommandResponseDto>> GetAllAsync();
        Task<TicketCommandResponseDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateTicketRequest request);
        Task<bool> UpdateAsync(int id, UpdateTicketRequest request);
        Task<bool> DeleteAsync(int id);
    }

    public class TicketService : ITicketService
    {
        private readonly ApiClient _apiClient;

        public TicketService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<TicketCommandResponseDto>> GetAllAsync()
        {
            return await _apiClient.GetAsync<List<TicketCommandResponseDto>>("/api/tickets") ?? new();
        }

        public async Task<TicketCommandResponseDto?> GetByIdAsync(int id)
        {
            return await _apiClient.GetAsync<TicketCommandResponseDto>($"/api/tickets/{id}");
        }

        public async Task<int> CreateAsync(CreateTicketRequest request)
        {
            return await _apiClient.PostAsync<CreateTicketRequest, int>(
                "/api/tickets",
                request
            );
        }

        public async Task<bool> UpdateAsync(int id, UpdateTicketRequest request)
        {
            return await _apiClient.PutAsync($"/api/tickets/{id}", request);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _apiClient.DeleteAsync($"/api/tickets/{id}");
        }
    }

    public class TicketCommandResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}