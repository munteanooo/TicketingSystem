using ClientUI.Models;

namespace ClientUI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticatedAsync();
        ValueTask<string?> GetTokenAsync();
        ValueTask<string?> GetUserIdAsync();
        ValueTask<string?> GetUserEmailAsync();
        ValueTask<string?> GetUserFullNameAsync();
        Task SetAuthDataAsync(string token, Guid userId, string email, string fullName); 
        Task LogoutAsync();
        Task SetAuthorizationHeaderAsync();
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(RegisterRequest request); 
    }
}