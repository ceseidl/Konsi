using Konsi.API.Services;
using Konsi.Shared;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Konsi.Worker
{
    public class RabbitMQConsumer : BackgroundService, IHostedService, IDisposable
    {
        private readonly IModel _channel;
        private readonly RedisCacheService _redisService;
        private readonly BeneficioService _beneficioService;
        private readonly ElasticsearchService _elasticService;

        public RabbitMQConsumer(IModel channel, RedisCacheService redisService, BeneficioService beneficioService, ElasticsearchService elasticService)
        {
            _channel = channel;
            _redisService = redisService;
            _beneficioService = beneficioService;
            _elasticService = elasticService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Implementation of the background service
            return Task.CompletedTask;
        }

        public async Task ProcessMessage(BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
        {
            var cpf = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var cachedData = await _redisService.GetAsync(cpf);

            if (cachedData != null)
            {
                // Data exists in Redis, no need to call API
                return;
            }

            var benefits = await _beneficioService.GetBenefitsAsync(cpf);
            await _elasticService.IndexBenefitsAsync(cpf, benefits);
            await _redisService.SetAsync(cpf, benefits);
        }
    }
}