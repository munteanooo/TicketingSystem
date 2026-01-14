using ClientUI.Models;

namespace ClientUI.Services
{
    public interface IUserService
    {
        Task<List<LoginCommandDto>> GetAllAsync();
        Task<LoginCommandDto?> GetByIdAsync(Guid id);
    }

    public class UserService : IUserService
    {
        private readonly ApiClient _apiClient;

        public UserService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<LoginCommandDto>> GetAllAsync()
        {
            return await _apiClient.GetAsync<List<LoginCommandDto>>("/api/users") ?? new();
        }

        public async Task<LoginCommandDto?> GetByIdAsync(Guid id)
        {
            return await _apiClient.GetAsync<LoginCommandDto>($"/api/users/{id}");
        }
    }
}
