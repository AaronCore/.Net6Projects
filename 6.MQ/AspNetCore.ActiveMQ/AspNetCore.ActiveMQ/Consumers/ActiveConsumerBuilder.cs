using Microsoft.Extensions.DependencyInjection;
using System;
using AspNetCore.ActiveMQ;
using AspNetCore.ActiveMQ.Consumers;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.Activemq.Consumers
{
    public class ActiveConsumerBuilder : IActiveConsumerBuilder
    {
        ActiveConsumerOptions activeConsumerOptions;

        public ActiveConsumerBuilder(IServiceCollection services, ActiveConsumerOptions activeConsumerOptions)
        {
            this.Services = services;
            this.activeConsumerOptions = activeConsumerOptions;
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public IActiveConsumerBuilder AddListener(Action<IServiceProvider, RecieveResult> onMessageRecieved)
        {
            Services.AddSingleton<IActiveConsumerProvider>(serviceProvider =>
            {
                return new DefaultActiveConsumerProvider(activeConsumerOptions, recieveResult =>
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        onMessageRecieved?.Invoke(serviceProvider.CreateScope().ServiceProvider, recieveResult);
                    }
                });
            });

            return this;
        }
    }
}
