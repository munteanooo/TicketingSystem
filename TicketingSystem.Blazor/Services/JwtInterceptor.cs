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
        // 1. Verificăm dacă request-ul nu este pentru Login/Register
        var isAuthRequest = request.RequestUri?.AbsolutePath.Contains("/api/auth/", StringComparison.OrdinalIgnoreCase) ?? false;

        if (!isAuthRequest)
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken", cancellationToken);

                if (!string.IsNullOrEmpty(token))
                {
                    // Curățăm ghilimelele reziduale dacă token-ul a fost salvat prin JSON.Serialize
                    token = token.Trim('"');
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                // În caz de eroare la citirea storage-ului, logăm eroarea
                Console.WriteLine($"Interceptor Error: {ex.Message}");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}