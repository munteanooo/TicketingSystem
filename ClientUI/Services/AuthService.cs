using System.Text.Json;
using Blazored.LocalStorage;
using ClientUI.Models;
using ClientUI.Services.Interfaces;

namespace ClientUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        // Login
        public async Task<string?> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "/api/auth/login",
                    new { email, password }
                );

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("token", out var tokenElement))
                {
                    var token = tokenElement.GetString();

                    // Extrage info din JWT (optional, pentru detalii)
                    if (root.TryGetProperty("userId", out var userIdElement) &&
                        root.TryGetProperty("email", out var emailElement) &&
                        root.TryGetProperty("fullName", out var fullNameElement))
                    {
                        var userId = userIdElement.GetString();
                        var userEmail = emailElement.GetString();
                        var fullName = fullNameElement.GetString();

                        await SetAuthDataAsync(token, Guid.Parse(userId), userEmail, fullName);
                    }
                    else
                    {
                        // Salvează doar tokenul dacă nu e alte date
                        await _localStorage.SetItemAsync("jwt_token", token);
                    }

                    return token;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login error: {ex.Message}");
                return null;
            }
        }

        // Register
        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "/api/auth/register",
                    request
                );

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Register error: {ex.Message}");
                return false;
            }
        }

        // Verifică autentificare
        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("jwt_token");
            return !string.IsNullOrWhiteSpace(token);
        }

        // Get Token
        public async ValueTask<string?> GetTokenAsync()
        {
            if (!OperatingSystem.IsBrowser()) return null;
            var token = await _localStorage.GetItemAsStringAsync("jwt_token");
            return token;
        }

        // Get User ID
        public async ValueTask<string?> GetUserIdAsync()
        {
            if (!OperatingSystem.IsBrowser()) return null;
            var userId = await _localStorage.GetItemAsStringAsync("user_id");
            return userId;
        }

        // Get User Email
        public async ValueTask<string?> GetUserEmailAsync()
        {
            if (!OperatingSystem.IsBrowser()) return null;
            var email = await _localStorage.GetItemAsStringAsync("user_email");
            return email;
        }

        // Get User Full Name
        public async ValueTask<string?> GetUserFullNameAsync()
        {
            if (!OperatingSystem.IsBrowser()) return null;
            var fullName = await _localStorage.GetItemAsStringAsync("user_fullName");
            return fullName;
        }

        // Set Auth Data (token + user info)
        public async Task SetAuthDataAsync(string token, Guid userId, string email, string fullName)
        {
            if (!OperatingSystem.IsBrowser()) return;

            await _localStorage.SetItemAsync("jwt_token", token);
            await _localStorage.SetItemAsync("user_id", userId.ToString());
            await _localStorage.SetItemAsync("user_email", email);
            await _localStorage.SetItemAsync("user_fullName", fullName);

            Console.WriteLine($"✅ Auth data saved - Email: {email}, UserId: {userId}");
        }

        // Set Authorization Header (pentru manual updates)
        public async Task SetAuthorizationHeaderAsync()
        {
            if (!OperatingSystem.IsBrowser()) return;

            var token = await GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Logout
        public async Task LogoutAsync()
        {
            if (!OperatingSystem.IsBrowser()) return;

            await _localStorage.RemoveItemAsync("jwt_token");
            await _localStorage.RemoveItemAsync("user_id");
            await _localStorage.RemoveItemAsync("user_email");
            await _localStorage.RemoveItemAsync("user_fullName");

            // Șterge header Authorization
            _httpClient.DefaultRequestHeaders.Authorization = null;

            Console.WriteLine("🚪 Logged out");
        }
    }
}