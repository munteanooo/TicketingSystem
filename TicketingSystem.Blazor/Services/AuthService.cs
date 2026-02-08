using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using TicketingSystem.Blazor.Services.Interfaces;

namespace TicketingSystem.Blazor.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(HttpClient httpClient,
                       AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> Register(string fullName, string email, string password, string role)
    {
        var registerModel = new
        {
            FullName = fullName,
            Email = email,
            Password = password,
            Role = role
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Login(string email, string password)
    {
        var loginModel = new { Email = email, Password = password };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    var customAuthStateProvider = (ApiAuthenticationStateProvider)_authenticationStateProvider;
                    await customAuthStateProvider.MarkUserAsAuthenticated(result.Token);

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);

                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task Logout()
    {
        var customAuthStateProvider = (ApiAuthenticationStateProvider)_authenticationStateProvider;
        await customAuthStateProvider.MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}