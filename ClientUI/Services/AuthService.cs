using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using ClientUI.Models;
using ClientUI.Services.Interfaces;

namespace ClientUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

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

        public ValueTask<string?> GetTokenAsync()
            => _localStorage.GetItemAsStringAsync("jwt_token");

        public ValueTask<string?> GetUserIdAsync()
            => _localStorage.GetItemAsStringAsync("user_id");

        public ValueTask<string?> GetUserEmailAsync()
            => _localStorage.GetItemAsStringAsync("user_email");

        public ValueTask<string?> GetUserFullNameAsync()
            => _localStorage.GetItemAsStringAsync("user_fullname");

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
                var dto = new LoginCommandDto
                {
                    Email = email,
                    Password = password
                };

                var json = System.Text.Json.JsonSerializer.Serialize(dto);
                Console.WriteLine($"LOGIN JSON: {json}");

                var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", dto);

                if (!response.IsSuccessStatusCode)
                    return false;

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse == null || !loginResponse.Success || loginResponse.User == null)
                    return false;

                await SetAuthDataAsync(
                    loginResponse.Token,
                    loginResponse.User.Id,
                    loginResponse.User.Email,
                    loginResponse.User.FullName
                );

                return true;
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
                var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", request);
                if (!response.IsSuccessStatusCode)
                    return false;

                var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                if (registerResponse == null || !registerResponse.Success)
                    return false;

                if (!string.IsNullOrEmpty(registerResponse.Token))
                {
                    await SetAuthDataAsync(
                        registerResponse.Token,
                        registerResponse.UserId,
                        registerResponse.Email,
                        registerResponse.FullName
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}");
                return false;
            }
        }
    }
}
