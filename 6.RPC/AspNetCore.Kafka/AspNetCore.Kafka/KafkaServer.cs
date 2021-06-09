using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Kafka
{
    public class KafkaServer : IDisposable
    {
        IServiceProvider serviceProvider;
        bool isStarted = false;
        bool disposed = false;

        public KafkaServer()
        {
            this.Services = new ServiceCollection();
            Services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        public IServiceCollection Services { get; private set; }
        public IServiceProvider ServiceProvider => isStarted ? serviceProvider : null;

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="configure"></param>
        public void Register(Action<IServiceCollection> configure)
        {
            if (isStarted)
            {
                throw new InvalidOperationException("server started");
            }
            configure?.Invoke(Services);
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (disposed)
            {
                throw new InvalidOperationException("server has disposed");
            }
            if (!isStarted)
            {
                isStarted = true;
                serviceProvider = Services.BuildServiceProvider();
                var hostedServices = serviceProvider.GetService<IEnumerable<IHostedService>>();
                foreach (var hostedService in hostedServices)
                {
                    await hostedService.StartAsync(cancellationToken);
                }
            }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (isStarted)
            {
                isStarted = false;

                var hostedServices = serviceProvider.GetService<IEnumerable<IHostedService>>();
                foreach (var hostedService in hostedServices)
                {
                    await hostedService.StopAsync(cancellationToken);
                }

                serviceProvider = null;
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                StopAsync().Wait();
            }
        }
    }
}
