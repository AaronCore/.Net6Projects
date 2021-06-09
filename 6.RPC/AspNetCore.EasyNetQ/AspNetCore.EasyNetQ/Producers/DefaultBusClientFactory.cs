using EasyNetQ;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Linq;

namespace AspNetCore.EasyNetQ.Producers
{
    public class DefaultBusClientFactory : IBusClientFactory
    {
        ConcurrentDictionary<string, IBusClient> busDictionary;
        IServiceProvider serviceProvider;

        public DefaultBusClientFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.busDictionary = new ConcurrentDictionary<string, IBusClient>();
        }

        /// <summary>
        /// 创建总线对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IBusClient Create(string name)
        {
            var optionsFactory = serviceProvider.GetService<IOptionsFactory<EasyNetQProducerOptions>>();
            var rabbitProducerOptions = optionsFactory.Create(name);
            if ((rabbitProducerOptions.Hosts == null || rabbitProducerOptions.Hosts.Length == 0) && string.IsNullOrEmpty(rabbitProducerOptions.ConnectionString))
            {
                throw new InvalidOperationException($"{nameof(EasyNetQOptions)} named '{name}' is not configured");
            }

            lock (busDictionary)
            {
                if (!busDictionary.TryGetValue(name, out IBusClient busClient) || busClient == null || !busClient.IsConnected)
                {
                    busClient = new BusClient(rabbitProducerOptions);
                    busDictionary.AddOrUpdate(name, busClient, (n, b) => busClient);
                }
                return busClient;
            }
        }

    }
}
