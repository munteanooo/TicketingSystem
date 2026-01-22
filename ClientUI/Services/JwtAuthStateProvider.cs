using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClientUI.Services
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public JwtAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetTokenAsync(string token)
        {
            if (!OperatingSystem.IsBrowser()) return; // Blazor prerendering

            await _localStorage.SetItemAsync("jwt_token", token);
            Console.WriteLine($"TOKEN SAVED IN LOCALSTORAGE: {token}");

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Client") }, "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task<string?> GetStoredTokenAsync()
        {
            if (!OperatingSystem.IsBrowser()) return null;

            var token = await _localStorage.GetItemAsStringAsync("jwt_token");
            return token;
        }

        public async Task LogoutAsync()
        {
            if (OperatingSystem.IsBrowser())
                await _localStorage.RemoveItemAsync("jwt_token");

            var anon = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anon)));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!OperatingSystem.IsBrowser())
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var token = await _localStorage.GetItemAsStringAsync("jwt_token");

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Client") }, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
