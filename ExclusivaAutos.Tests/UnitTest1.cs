using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using ExclusivaAutos.Application.Services;
using ExclusivaAutos.Domain.Interfaces;
using ExclusivaAutos.Domain.Entities;

namespace ExclusivaAutos.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<ILogger<CustomerService>> _mockLogger;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockLogger = new Mock<ILogger<CustomerService>>();
            _customerService = new CustomerService(_mockCustomerService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCustomerByDocumentAsync_ValidDocument_ReturnsCustomer()
        {
            var testDocument = "12345678";
            var expectedCustomer = new Customer
            {
                FirstName = "Jose",
                LastName = "Ortiz",
                Email = "Jose.Ortiz@email.com",
                Age = "33",
                City = "Bogota"
            };

            _mockCustomerService
                .Setup(s => s.GetCustomerByDocumentAsync(testDocument))
                .ReturnsAsync(expectedCustomer);

            var result = await _customerService.GetCustomerByDocumentAsync(testDocument);

            Assert.NotNull(result);
            Assert.Equal(expectedCustomer.FirstName, result.FirstName);
            Assert.Equal(expectedCustomer.LastName, result.LastName);
            Assert.Equal(expectedCustomer.Email, result.Email);
            Assert.Equal(expectedCustomer.Age, result.Age);
            Assert.Equal(expectedCustomer.City, result.City);
        }

        [Fact]
        public async Task GetCustomerByDocumentAsync_EmptyDocument_ThrowsArgumentException()
        {
            var emptyDocument = "";

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _customerService.GetCustomerByDocumentAsync(emptyDocument));
        }

        [Fact]
        public async Task GetCustomerByDocumentAsync_NullDocument_ThrowsArgumentException()
        {
            string nullDocument = null;

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _customerService.GetCustomerByDocumentAsync(nullDocument));
        }

        [Fact]
        public async Task GetCustomerByDocumentAsync_ServiceThrowsException_PropagatesException()
        {
            var testDocument = "12345678";
            var expectedException = new Exception("Test exception");

            _mockCustomerService
                .Setup(s => s.GetCustomerByDocumentAsync(testDocument))
                .ThrowsAsync(expectedException);

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _customerService.GetCustomerByDocumentAsync(testDocument));

            Assert.Equal(expectedException.Message, exception.Message);
        }
    }
}