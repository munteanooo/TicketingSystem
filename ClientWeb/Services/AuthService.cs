using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using ClientWeb.Models;
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
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                return jwtToken?.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
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

        public async Task SetAuthDataAsync(string token, int userId, string email)
        {
            await _localStorage.SetItemAsync("jwt_token", token);
            await _localStorage.SetItemAsync("user_id", userId.ToString());
            await _localStorage.SetItemAsync("user_email", email);
            await SetAuthorizationHeaderAsync();
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("jwt_token");
            await _localStorage.RemoveItemAsync("user_id");
            await _localStorage.RemoveItemAsync("user_email");
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
                var request = new LoginRequest { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(json, _jsonOptions);

                    if (loginResponse?.Success == true && loginResponse.User != null)
                    {
                        await SetAuthDataAsync(loginResponse.Token, loginResponse.User.Id, loginResponse.User.Email);
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

        public async Task<bool> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            try
            {
                var request = new RegisterRequest
                {
                    Email = email,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName
                };

                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/register", request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var registerResponse = JsonSerializer.Deserialize<RegisterResponse>(json, _jsonOptions);
                    return registerResponse?.Success ?? false;
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