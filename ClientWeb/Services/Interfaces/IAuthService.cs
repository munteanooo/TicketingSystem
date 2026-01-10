using ClientWeb.Models;

namespace ClientWeb.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<string?> GetUserIdAsync();
        Task<string?> GetUserEmailAsync();
        Task<string?> GetUserFullNameAsync(); 
        Task SetAuthDataAsync(string token, Guid userId, string email, string fullName); 
        Task LogoutAsync();
        Task SetAuthorizationHeaderAsync();
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(RegisterRequest request); 
    }
}