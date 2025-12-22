namespace ClientWeb.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<string?> GetUserIdAsync();
        Task<string?> GetUserEmailAsync();
        Task SetAuthDataAsync(string token, int userId, string email);
        Task LogoutAsync();
        Task SetAuthorizationHeaderAsync();
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password, string firstName, string lastName);
    }
}