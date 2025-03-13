using Konsi.API.Models;
using Konsi.Shared;
using Moq;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Konsi.Tests
{
    public class ElasticsearchServiceTests
    {
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly ElasticsearchService _elasticService;

        public ElasticsearchServiceTests()
        {
            _elasticClientMock = new Mock<IElasticClient>();
            _elasticService = new ElasticsearchService(_elasticClientMock.Object);
        }

        [Fact]
        public async Task IndexBenefits_ShouldIndexDataCorrectly()
        {
            // Arrange
            var cpf = "12345678900";
            var beneficios = new List<Beneficio>
        {
            new Beneficio { Number = "12345", Type = "Aposentadoria" }
        };

            // Act
            await _elasticService.IndexBenefitsAsync(cpf, beneficios);

            // Assert
            _elasticClientMock.Verify(e => e.IndexDocumentAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}