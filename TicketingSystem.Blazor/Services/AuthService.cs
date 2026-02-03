using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TicketingSystem.Application.Contracts.Identity;

namespace TicketingSystem.Blazor.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient,
                       ILocalStorageService localStorage,
                       AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> Login(string email, string password)
    {
        var loginRequest = new LoginRequest { Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                await _localStorage.SetItemAsync("authToken", result.Token);

                var customAuthStateProvider = (ApiAuthenticationStateProvider)_authStateProvider;
                customAuthStateProvider.MarkUserAsAuthenticated(result.Token);

                return true;
            }
        }
        return false;
    }

    public async Task<bool> Register(string fullName, string email, string password)
    {
        var registerRequest = new { FullName = fullName, Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
        return response.IsSuccessStatusCode;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        var customAuthStateProvider = (ApiAuthenticationStateProvider)_authStateProvider;
        customAuthStateProvider.MarkUserAsLoggedOut();
    }
}