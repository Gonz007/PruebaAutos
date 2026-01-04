using ExclusivaAutos.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ExclusivaAutos.Application.DTOs;

namespace ExclusivaAutos.Api.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerApplitacionService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerApplitacionService customerService,
            ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet("{document}")]
        [ProducesResponseType(typeof(CustomerDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(502)]
        [ProducesResponseType(504)]
        public async Task<IActionResult> GetCustomerByDocument(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                return BadRequest("El número de documento es requerido");

            try
            {
                var customer = await _customerService.GetCustomerByDocumentAsync(document);

                if (customer == null)
                    return NotFound($"No se encontró cliente con documento: {document}");

                return Ok(customer);
            }
            catch (TimeoutException ex)
            {
                _logger.LogWarning(ex, "Timeout para documento: {Document}", document);
                return StatusCode(504, "Timeout al comunicarse con el servicio externo");
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                _logger.LogError(ex, "Error HTTP {StatusCode} para documento: {Document}",
                    ex.StatusCode, document);

                return StatusCode((int)ex.StatusCode.Value, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de comunicación con servicio externo para documento: {Document}", document);
                return StatusCode(502, "Error al comunicarse con el servicio externo");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno para documento: {Document}", document);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
