using AspNetCore.RabbitMQ.Integration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Consumers
{
    public class RabbitConsumerBuilder : IRabbitConsumerBuilder
    {
        RabbitConsumerOptions rabbitConsumerOptions;

        public RabbitConsumerBuilder(IServiceCollection services, RabbitConsumerOptions rabbitConsumerOptions)
        {
            this.Services = services;
            this.rabbitConsumerOptions = rabbitConsumerOptions;
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// 添加队列监听
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public IRabbitConsumerBuilder AddListener(string queue, Action<IServiceProvider, RecieveResult> onMessageRecieved)
        {
            if (string.IsNullOrEmpty(queue))
            {
                throw new ArgumentException($"queue cann't be empty", nameof(queue));
            }

            Services.AddSingleton<IRabbitConsumerProvider>(serviceProvider =>
            {
                return new DefaultRabbitConsumerProvider(queue, rabbitConsumerOptions, result =>
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        onMessageRecieved?.Invoke(scope.ServiceProvider, result);
                    }
                });
            });

            return this;
        }
        /// <summary>
        /// 添加交换机监听
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public IRabbitConsumerBuilder AddListener(string exchange, string queue, Action<IServiceProvider, RecieveResult> onMessageRecieved)
        {
            if (string.IsNullOrEmpty(exchange))
            {
                throw new ArgumentException($"exchange cann't be empty", nameof(exchange));
            }
            if (string.IsNullOrEmpty(queue))
            {
                throw new ArgumentException($"queue cann't be empty", nameof(queue));
            }
            Services.AddSingleton<IRabbitConsumerProvider>(serviceProvider =>
            {
                return new DefaultRabbitConsumerProvider(exchange, queue, rabbitConsumerOptions, result =>
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        onMessageRecieved?.Invoke(scope.ServiceProvider, result);
                    }
                });
            });

            return this;
        }
    }
}
