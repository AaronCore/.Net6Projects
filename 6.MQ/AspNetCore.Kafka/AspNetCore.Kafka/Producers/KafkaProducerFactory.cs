using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Kafka.Producers
{
    public class KafkaProducerFactory : IKafkaProducerFactory
    {
        ConcurrentDictionary<string, IKafkaClientProducer> clientProducers;
        IServiceProvider serviceProvider;

        public KafkaProducerFactory(IServiceProvider serviceProvider)
        {
            this.clientProducers = new ConcurrentDictionary<string, IKafkaClientProducer>();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 创建客户端生产者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IKafkaClientProducer Create(string name)
        {
            var optionsFactory = serviceProvider.GetService<IOptionsFactory<KafkaProducerOptions>>();
            var kafkaProducerOptions = optionsFactory.Create(name);
            if (kafkaProducerOptions.BootstrapServers == null || kafkaProducerOptions.BootstrapServers.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(KafkaProducerOptions)} named '{name}' is not configured");
            }

            lock (clientProducers)
            {
                if (!clientProducers.TryGetValue(name, out IKafkaClientProducer clientProducer) || (clientProducer as DefaultKafkaClientProducer).Disposed)
                {
                    clientProducer = new DefaultKafkaClientProducer(kafkaProducerOptions);
                    clientProducers.AddOrUpdate(name, clientProducer, (n, b) => clientProducer);
                }
                return clientProducer;
            }
        }
    }
}
