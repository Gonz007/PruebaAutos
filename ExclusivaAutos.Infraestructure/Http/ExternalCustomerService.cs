using System.Net;
using System.Text;
using System.Text.Json;
using ExclusivaAutos.Domain.Entities;
using ExclusivaAutos.Domain.Interfaces;
using ExclusivaAutos.Infraestructure.Configuration;
using ExclusivaAutos.Infraestructure.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExclusivaAutos.Infrastructure.Http
{
    public class ExternalCustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalApiSettings _settings;
        private readonly IOAuthTokenService _tokenService;
        private readonly ILogger<ExternalCustomerService> _logger;

        public ExternalCustomerService(
            HttpClient httpClient,
            IOptions<ExternalApiSettings> settings,
            IOAuthTokenService tokenService,
            ILogger<ExternalCustomerService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _tokenService = tokenService;
            _logger = logger;

            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        }

        public async Task<Customer> GetCustomerByDocumentAsync(string document)
        {
            try
            {
                var token = await _tokenService.GetAccessTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var requestBody = new { CustomerId = document };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Consultando cliente con documento: {Document}", document);

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpRequestException("Unauthorized", null, HttpStatusCode.Unauthorized);
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Error en la API externa: {response.StatusCode}",
                        null,
                        response.StatusCode);
                }


                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var customerResponse = JsonSerializer.Deserialize<CustomerResponse>(responseContent, options);

                return Customer.FromResponse(
                    customerResponse.FirstName,
                    customerResponse.LastName,
                    customerResponse.Email,
                    customerResponse.Age,
                    customerResponse.City
                );
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Timeout al consultar cliente con documento: {Document}", document);
                throw new TimeoutException($"La consulta para el documento {document} ha excedido el tiempo límite");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar cliente con documento: {Document}", document);
                throw;
            }
        }

    }
}