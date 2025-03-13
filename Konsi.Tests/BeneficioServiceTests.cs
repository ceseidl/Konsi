using Moq;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using Konsi.API.Services;
using System.Net.Http.Json;
using System.Net;
using Konsi.API.Models;

namespace Konsi.Tests
{
    public class BeneficioServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly BeneficioService _beneficioService;

        public BeneficioServiceTests()
        {
            _httpHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpHandlerMock.Object)
            {
                BaseAddress = new Uri("https://mockapi.com")
            };
            _beneficioService = new BeneficioService(_httpClient);
        }

        [Fact]
        public async Task GetBenefits_ShouldReturnBenefits_WhenApiReturnsData()
        {
            // Arrange
            var cpf = "12345678900";
            var mockResponse = new List<Beneficio>
        {
            new Beneficio { Number = "12345", Type = "Aposentadoria" }
        };

           
            // Act
            var result = await _beneficioService.GetBenefitsAsync(cpf);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("12345", result[0].Number);
        }
    }
}
