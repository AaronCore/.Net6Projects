using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AspNetCore.ActiveMQ;
using AspNetCore.ActiveMQ.Producers;

namespace AspNetCore.Activemq.Producers
{
    public class DefaultActiveProducerFactory : IActiveProducerFactory
    {
        ConcurrentDictionary<string, IActiveClientProducer> clientProducers;
        IServiceProvider serviceProvider;

        public DefaultActiveProducerFactory(IServiceProvider serviceProvider)
        {
            this.clientProducers = new ConcurrentDictionary<string, IActiveClientProducer>();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 创建一个生产者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActiveClientProducer Create(string name)
        {
            var optionsFactory = serviceProvider.GetService<IOptionsFactory<ActiveProducerOptions>>();
            var activeProducerOptions = optionsFactory.Create(name);
            if (activeProducerOptions.BrokerUris == null || activeProducerOptions.BrokerUris.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(ActiveProducerOptions)} named '{name}' is not configured");
            }

            lock (clientProducers)
            {
                if (!clientProducers.TryGetValue(name, out IActiveClientProducer clientProducer) || (clientProducer as DefaultActiveClientProducer).Disposed)
                {
                    clientProducer = new DefaultActiveClientProducer(activeProducerOptions);
                    clientProducers.AddOrUpdate(name, clientProducer, (n, b) => clientProducer);
                }
                return clientProducer;
            }
        }
    }
}
