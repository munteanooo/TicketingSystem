using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace TicketingSystem.Blazor.Services;

public class JwtInterceptor : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public JwtInterceptor(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}