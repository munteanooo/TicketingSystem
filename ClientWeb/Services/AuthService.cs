using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;
using ClientWeb.Services.Interfaces;

namespace ClientWeb.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private const string API_BASE_URL = "/api/auth";
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthService(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsStringAsync("jwt_token");
        }

        public async Task<string?> GetUserIdAsync()
        {
            return await _localStorage.GetItemAsStringAsync("user_id");
        }

        public async Task<string?> GetUserEmailAsync()
        {
            return await _localStorage.GetItemAsStringAsync("user_email");
        }

        public async Task<string?> GetUserFullNameAsync()
        {
            return await _localStorage.GetItemAsStringAsync("user_fullname");
        }

        public async Task SetAuthDataAsync(string token, Guid userId, string email, string fullName)
        {
            await _localStorage.SetItemAsync("jwt_token", token);
            await _localStorage.SetItemAsync("user_id", userId.ToString());
            await _localStorage.SetItemAsync("user_email", email);
            await _localStorage.SetItemAsync("user_fullname", fullName);
            await SetAuthorizationHeaderAsync();
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("jwt_token");
            await _localStorage.RemoveItemAsync("user_id");
            await _localStorage.RemoveItemAsync("user_email");
            await _localStorage.RemoveItemAsync("user_fullname");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task SetAuthorizationHeaderAsync()
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var request = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    var success = doc.RootElement.GetProperty("success").GetBoolean();
                    if (success)
                    {
                        var token = doc.RootElement.GetProperty("token").GetString();
                        var user = doc.RootElement.GetProperty("user");

                        // Parsează userId ca Guid
                        var userIdString = user.GetProperty("id").GetString();
                        if (!Guid.TryParse(userIdString, out var userId))
                        {
                            return false;
                        }

                        var userEmail = user.GetProperty("email").GetString();
                        var fullName = user.GetProperty("fullName").GetString();

                        await SetAuthDataAsync(token, userId, userEmail, fullName);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Trimite RegisterRequest direct la API
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

                if (response.IsSuccessStatusCode)
                {
                    // Backend-ul returnează success: true/false
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    return doc.RootElement.GetProperty("success").GetBoolean();
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}");
                return false;
            }
        }
    }
}