using Blazored.LocalStorage;

namespace ClientUI.Services
{
    public class AuthHttpClientHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHttpClientHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!OperatingSystem.IsBrowser())
                return await base.SendAsync(request, cancellationToken);

            // Ia tokenul din localStorage
            var token = await _localStorage.GetItemAsStringAsync("jwt_token");

            // Adaugă header Authorization pe FIECARE cerere
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine($"🔑 Token adăugat în header: {token.Substring(0, 20)}...");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}