using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Kafka.Consumers
{
    public class DefaultConsumerHostedService : IHostedService
    {
        ILoggerFactory loggerFactory;
        IEnumerable<IKafkaConsumerProvider> kafkaConsumerProviders;
        public DefaultConsumerHostedService(ILoggerFactory loggerFactory, IEnumerable<IKafkaConsumerProvider> kafkaConsumerProviders)
        {
            this.kafkaConsumerProviders = kafkaConsumerProviders;
            this.loggerFactory = loggerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var logger = loggerFactory.CreateLogger<DefaultConsumerHostedService>();

            foreach (var provider in kafkaConsumerProviders)
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

                foreach (var provider in kafkaConsumerProviders)
                {
                    provider.Dispose();

                    logger.LogInformation($"Consumer Stoped:{provider}");
                }
            });
            await Task.CompletedTask;
        }
    }
}
