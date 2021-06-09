using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ActiveMQ.Consumers
{
    public class DefaultConsumerHostedService : IHostedService
    {
        ILoggerFactory loggerFactory;
        IEnumerable<IActiveConsumerProvider> activeConsumerProviders;
        public DefaultConsumerHostedService(ILoggerFactory loggerFactory, IEnumerable<IActiveConsumerProvider> activeConsumerProviders)
        {
            this.activeConsumerProviders = activeConsumerProviders;
            this.loggerFactory = loggerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var logger = loggerFactory.CreateLogger<DefaultConsumerHostedService>();

            foreach (var provider in activeConsumerProviders)
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

                foreach (var provider in activeConsumerProviders)
                {
                    provider.Dispose();

                    logger.LogInformation($"Consumer Stoped:{provider}");
                }
            });
            await Task.CompletedTask;
        }
    }
}
