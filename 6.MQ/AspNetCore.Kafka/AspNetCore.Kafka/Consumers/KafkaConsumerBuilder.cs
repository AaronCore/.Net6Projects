using AspNetCore.Kafka.Integration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Kafka.Consumers
{
    public class KafkaConsumerBuilder : IKafkaConsumerBuilder
    {
        KafkaConsumerOptions kafkaConsumerOptions;

        public KafkaConsumerBuilder(IServiceCollection services, KafkaConsumerOptions kafkaConsumerOptions)
        {
            this.Services = services;
            this.kafkaConsumerOptions = kafkaConsumerOptions;
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// 添加队列监听
        /// </summary>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public IKafkaConsumerBuilder AddListener(Action<IServiceProvider, RecieveResult> onMessageRecieved)
        {
            Services.AddSingleton<IKafkaConsumerProvider>(serviceProvider =>
            {
                return new DefaultKafkaConsumerProvider(kafkaConsumerOptions, result =>
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
