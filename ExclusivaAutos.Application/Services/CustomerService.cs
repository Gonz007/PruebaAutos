using System;
using System.Threading.Tasks;
using ExclusivaAutos.Application.DTOs;
using ExclusivaAutos.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ExclusivaAutos.Application.Services
{
    public class CustomerService : ICustomerApplitacionService
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerService customerService, ILogger<CustomerService> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<CustomerDto> GetCustomerByDocumentAsync(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
            {
                throw new ArgumentException("El documento no puede estar vacío", nameof(document));
            }

            try
            {
                _logger.LogInformation("Iniciando consulta para documento: {Document}", document);

                var customer = await _customerService.GetCustomerByDocumentAsync(document);
                if (customer == null)
                    return null;
                return new CustomerDto
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Age = customer.Age,
                    City = customer.City
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la aplicación al consultar documento: {Document}", document);
                throw;
            }
        }
    }
}