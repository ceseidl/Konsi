using Konsi.API.Models;
using Konsi.API.Services;
using Konsi.Shared;
using Konsi.Worker;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Timers;
using Xunit;

namespace Konsi.Tests
{
    public class RabbitMQConsumerTests
    {
        private readonly Mock<IModel> _channelMock;
        private readonly Mock<RedisCacheService> _redisServiceMock;
        private readonly Mock<BeneficioService> _beneficioServiceMock;
        private readonly Mock<ElasticsearchService> _elasticServiceMock;
        private readonly RabbitMQConsumer _consumer;

        public RabbitMQConsumerTests()
        {
            _channelMock = new Mock<IModel>();
            _redisServiceMock = new Mock<RedisCacheService>();
            _beneficioServiceMock = new Mock<BeneficioService>();
            _elasticServiceMock = new Mock<ElasticsearchService>();

            _consumer = new RabbitMQConsumer(
                _channelMock.Object,
                _redisServiceMock.Object,
                _beneficioServiceMock.Object,
                _elasticServiceMock.Object
            );
        }

        [Fact]
        public async void Consumer_ShouldNotCallApi_IfDataExistsInRedis()
        {
            // Arrange
            var cpf = "12345678900";
            var messageBody = Encoding.UTF8.GetBytes(cpf);
            var eventArgs = new BasicDeliverEventArgs(
                consumerTag: "consumerTag",
                deliveryTag: 1,
                redelivered: false,
                exchange: "exchange",
                routingKey: "routingKey",
                properties: null,
                body: messageBody
            );

            _redisServiceMock.Setup(r => r.GetAsync(cpf)).ReturnsAsync("{\"benefit\": \"mockData\"}");

            // Act
            var token = new CancellationToken();
            await _consumer.ProcessMessage(eventArgs, token);

            // Assert
            _beneficioServiceMock.Verify(b => b.GetBenefitsAsync(It.IsAny<string>()), Times.Never);
            _elasticServiceMock.Verify(e => e.IndexBenefitsAsync(It.IsAny<string>(), It.IsAny<List<Beneficio>>()), Times.Never);
        }

        [Fact]
        public async void Consumer_ShouldCallApi_IfDataDoesNotExistInRedis()
        {
            // Arrange
            var cpf = "12345678900";
            var messageBody = Encoding.UTF8.GetBytes(cpf);
            var eventArgs = new BasicDeliverEventArgs(
                consumerTag: "consumerTag",
                deliveryTag: 1,
                redelivered: false,
                exchange: "exchange",
                routingKey: "routingKey",
                properties: null,
                body: messageBody
            );

            _redisServiceMock.Setup(r => r.GetAsync(cpf)).ReturnsAsync((string)null);
            _beneficioServiceMock.Setup(b => b.GetBenefitsAsync(cpf)).ReturnsAsync(new List<Beneficio>
        {
            new Beneficio { Number = "12345", Type = "Aposentadoria" }
        });

            // Act
            var token = new CancellationToken();
            await _consumer.ProcessMessage(eventArgs, token);

            // Assert
            _beneficioServiceMock.Verify(b => b.GetBenefitsAsync(cpf), Times.Once);
            _elasticServiceMock.Verify(e => e.IndexBenefitsAsync(cpf, It.IsAny<List<Beneficio>>()), Times.Once);
            _redisServiceMock.Verify(r => r.SetAsync(cpf, It.IsAny<List<Beneficio>>()), Times.Once);
        }

        [Fact]
        public async void Consumer_ShouldHandleExceptionsGracefully()
        {
            // Arrange
            var cpf = "12345678900";
            var messageBody = Encoding.UTF8.GetBytes(cpf);
            var eventArgs = new BasicDeliverEventArgs(
                consumerTag: "consumerTag",
                deliveryTag: 1,
                redelivered: false,
                exchange: "exchange",
                routingKey: "routingKey",
                properties: null,
                body: messageBody
            );

            _redisServiceMock.Setup(r => r.GetAsync(cpf)).ThrowsAsync(new Exception("Redis error"));

            // Act
            var token = new CancellationToken();
            await _consumer.ProcessMessage(eventArgs, token);

            // Assert
            _beneficioServiceMock.Verify(b => b.GetBenefitsAsync(It.IsAny<string>()), Times.Never);
            _elasticServiceMock.Verify(e => e.IndexBenefitsAsync(It.IsAny<string>(), It.IsAny<List<Beneficio>>()), Times.Never);
        }
    }
}