using System.Net.Http.Json;

namespace ClientWeb.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET error: {Endpoint}", endpoint);
                return default;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
    string endpoint,
    TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST error: {Endpoint}", endpoint);
                return default;
            }
        }

        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PUT error: {Endpoint}", endpoint);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DELETE error: {Endpoint}", endpoint);
                return false;
            }
        }
    }
}