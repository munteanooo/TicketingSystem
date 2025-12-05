using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Web.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<ClaimsPrincipal> GetUserAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;

        public AuthService(HttpClient httpClient,
                          AuthenticationStateProvider authStateProvider,
                          NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                // TODO: Înlocuiește cu API-ul real
                // var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { email, password });

                // Simulare pentru demo (șterge în producție)
                await Task.Delay(800);

                // Validare demo
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return new LoginResult
                    {
                        Success = false,
                        ErrorMessage = "Email and password are required"
                    };
                }

                if (email == "admin@example.com" && password == "admin123")
                {
                    // Create claims for admin
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim("FullName", "Administrator"),
                        new Claim("UserId", Guid.NewGuid().ToString())
                    };

                    await SetAuthenticationState(claims);

                    return new LoginResult
                    {
                        Success = true,
                        Token = "demo-token-admin",
                        Expiry = DateTime.UtcNow.AddHours(8)
                    };
                }
                else if (password.Length >= 6)
                {
                    // Create claims for regular user
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("FullName", email.Split('@')[0]),
                        new Claim("UserId", Guid.NewGuid().ToString())
                    };

                    await SetAuthenticationState(claims);

                    return new LoginResult
                    {
                        Success = true,
                        Token = "demo-token-user",
                        Expiry = DateTime.UtcNow.AddHours(8)
                    };
                }
                else
                {
                    return new LoginResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid email or password"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = $"Login failed: {ex.Message}"
                };
            }
        }

        private async Task SetAuthenticationState(Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims, "custom");
            var user = new ClaimsPrincipal(identity);

            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                customProvider.SetAuthenticationState(user);
            }

            // Notify state change
            await _authStateProvider.GetAuthenticationStateAsync();
        }

        public async Task LogoutAsync()
        {
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                customProvider.ClearAuthenticationState();
            }

            // Așteaptă schimbarea stării
            await _authStateProvider.GetAuthenticationStateAsync();

            // Navigate to login
            _navigationManager.NavigateTo("/login");
        }

        public async Task<ClaimsPrincipal> GetUserAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiry { get; set; }
    }
}