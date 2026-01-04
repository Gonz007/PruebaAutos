using System.Text.Json;
using ExclusivaAutos.Infraestructure.Configuration;
using ExclusivaAutos.Infraestructure.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExclusivaAutos.Infrastructure.Http
{
    public class OAuthTokenService : IOAuthTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalApiSettings _settings;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<OAuthTokenService> _logger;
        private string _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public OAuthTokenService(HttpClient httpClient,
            IOptions<ExternalApiSettings> settings,
            IEncryptionService encryptionService,
            ILogger<OAuthTokenService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
                return _cachedToken;

            try
            {
                var tokenEndpoint = $"{_settings.Authority}/oauth2/v2.0/token";

                var clientId = _encryptionService.Decrypt(_settings.ClientId);
                var clientSecret = _encryptionService.Decrypt(_settings.ClientSecret);

                var formData = new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["scope"] = _settings.Scope,
                    ["grant_type"] = "client_credentials"
                };

                var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
                {
                    Content = new FormUrlEncodedContent(formData)
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(responseContent, options);

                _cachedToken = tokenResponse.AccessToken;
                _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // 60 segundos de margen

                return _cachedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo token OAuth");
                throw;
            }
        }


    }
}