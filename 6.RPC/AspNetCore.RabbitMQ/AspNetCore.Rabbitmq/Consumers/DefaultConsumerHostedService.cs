using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.RabbitMQ.Consumers
{
    public class DefaultConsumerHostedService : IHostedService
    {
        ILoggerFactory loggerFactory;
        IEnumerable<IRabbitConsumerProvider> rabbitConsumerProviders;
        public DefaultConsumerHostedService(ILoggerFactory loggerFactory, IEnumerable<IRabbitConsumerProvider> rabbitConsumerProviders)
        {
            this.rabbitConsumerProviders = rabbitConsumerProviders;
            this.loggerFactory = loggerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var logger = loggerFactory.CreateLogger<DefaultConsumerHostedService>();

            foreach (var provider in rabbitConsumerProviders)
            {
                await provider.ListenAsync();

                logger.LogInformation($"Consumer Listen:{provider}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
            {
                var logger = loggerFactory.CreateLogger<DefaultConsumerHostedService>();

                foreach (var provider in rabbitConsumerProviders)
                {
                    provider.Dispose();

                    logger.LogInformation($"Consumer Stoped:{provider}");
                }
            });
            await Task.CompletedTask;
        }
    }
}
