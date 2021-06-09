using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.RabbitMQ
{
    public class RabbitProducerFactory : IRabbitProducerFactory
    {
        ConcurrentDictionary<string, IRabbitClientProducer> clientProducers;
        IServiceProvider serviceProvider;

        public RabbitProducerFactory(IServiceProvider serviceProvider)
        {
            this.clientProducers = new ConcurrentDictionary<string, IRabbitClientProducer>();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 创建客户端发送
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IRabbitClientProducer Create(string name)
        {
            var optionsFactory = serviceProvider.GetService<IOptionsFactory<RabbitProducerOptions>>();
            var rabbitProducerOptions = optionsFactory.Create(name);
            if (rabbitProducerOptions.Hosts == null || rabbitProducerOptions.Hosts.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(RabbitProducerOptions)} named '{name}' is not configured");
            }

            lock (clientProducers)
            {
                if (!clientProducers.TryGetValue(name, out IRabbitClientProducer rabbitClientProducer) || (rabbitClientProducer as RabbitClientProducer).Disposed)
                {
                    rabbitClientProducer = new RabbitClientProducer(rabbitProducerOptions);
                    clientProducers.AddOrUpdate(name, rabbitClientProducer, (n, b) => rabbitClientProducer);
                }
                return rabbitClientProducer;
            }
        }
    }
}
