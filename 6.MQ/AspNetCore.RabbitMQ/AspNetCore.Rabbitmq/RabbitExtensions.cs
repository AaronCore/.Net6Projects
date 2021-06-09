using AspNetCore.RabbitMQ.Consumers;
using AspNetCore.RabbitMQ.Integration;
using AspNetCore.RabbitMQ.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.RabbitMQ
{
    public static class RabbitExtensions
    {
        /// <summary>
        /// 注册添加一个生产者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitProducer(this IServiceCollection services, Action<RabbitProducerOptions> configure)
        {
            return services.AddRabbitProducer(Options.DefaultName, configure);
        }
        /// <summary>
        /// 注册添加一个生产者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitProducer(this IServiceCollection services, string name, Action<RabbitProducerOptions> configure)
        {
            services.TryAddSingleton<IRabbitProducerFactory, RabbitProducerFactory>();
            services.Configure(name, configure);
            return services;
        }

        /// <summary>
        /// 创建客户端生产者
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IRabbitClientProducer Create(this IRabbitProducerFactory factory)
        {
            return factory.Create(Options.DefaultName);
        }
        /// <summary>
        /// 普通的往队列发送消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="message"></param>
        public static void Publish(this IRabbitClientProducer producer, string message)
        {
            producer.Publish(new string[] { message });
        }
        /// <summary>
        /// 使用交换机发送消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        public static void Publish(this IRabbitClientProducer producer, string routingKey, string message)
        {
            producer.Publish(routingKey, new string[] { message });
        }
        /// <summary>
        /// 普通的往队列发送消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task PublishAsync(this IRabbitClientProducer producer, string message)
        {
            await producer.PublishAsync(new string[] { message });
        }
        /// <summary>
        /// 使用交换机发送消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="routingKey"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static async Task PublishAsync(this IRabbitClientProducer producer, string routingKey, string message)
        {
            await producer.PublishAsync(routingKey, new string[] { message });
        }
        /// <summary>
        /// 普通的往队列发送消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="messages"></param>
        public static async Task PublishAsync(this IRabbitClientProducer producer, string[] messages)
        {
            await Task.Run(() =>
            {
                producer.Publish(messages);
            });
        }
        /// <summary>
        /// 使用交换机发送消息
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="messages"></param>
        public static async Task PublishAsync(this IRabbitClientProducer producer, string routingKey, string[] messages)
        {
            await Task.Run(() =>
            {
                producer.Publish(routingKey, messages);
            });
        }

        /// <summary>
        /// 注册消费者服务对象
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitConsumerCore(this IServiceCollection services)
        {
            if (!services.Any(f => f.ImplementationType == typeof(DefaultConsumerHostedService)))
            {
                services.AddSingleton<IHostedService, DefaultConsumerHostedService>();
            }
            return services;
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitConsumer(this IServiceCollection services, Type serviceType)
        {
            if (!typeof(IRabbitConsumerProvider).IsAssignableFrom(serviceType) || !serviceType.IsClass || serviceType.IsAbstract)
            {
                throw new InvalidOperationException($"serviceType must be implement {nameof(IRabbitConsumerProvider)} and not abstract class ");
            }

            services.AddRabbitConsumerCore();
            services.AddSingleton(serviceType);
            return services;
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitConsumer<T>(this IServiceCollection services) where T : class, IRabbitConsumerProvider
        {
            return services.AddRabbitConsumer(typeof(T));
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rabbitConsumerProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitConsumer(this IServiceCollection services, IRabbitConsumerProvider rabbitConsumerProvider)
        {
            services.AddRabbitConsumerCore();
            services.AddSingleton(rabbitConsumerProvider);
            return services;
        }
        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddRabbitConsumer(this IServiceCollection services, Action<RabbitConsumerOptions> configure)
        {
            RabbitConsumerOptions rabbitConsumerOptions = new RabbitConsumerOptions();
            configure?.Invoke(rabbitConsumerOptions);
            return services.AddRabbitConsumer(rabbitConsumerOptions);
        }
        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rabbitConsumerOptions"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddRabbitConsumer(this IServiceCollection services, RabbitConsumerOptions rabbitConsumerOptions)
        {
            services.AddRabbitConsumerCore();
            return new RabbitConsumerBuilder(services, rabbitConsumerOptions);
        }

        /// <summary>
        /// 添加队列监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="queue"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddListener(this IRabbitConsumerBuilder builder, string queue, Action<RecieveResult> onMessageRecieved)
        {
            return builder.AddListener(queue, (_, result) =>
            {
                onMessageRecieved?.Invoke(result);
            });
        }
        /// <summary>
        /// 添加交换机监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddListener(this IRabbitConsumerBuilder builder, string exchange, string queue, Action<RecieveResult> onMessageRecieved)
        {
            return builder.AddListener(exchange, queue, (_, result) =>
            {
                onMessageRecieved?.Invoke(result);
            });
        }
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="queue"></param>
        /// <param name="listenerType"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddListener(this IRabbitConsumerBuilder builder, string queue, Type listenerType)
        {
            if (!typeof(IRabbitConsumerListener).IsAssignableFrom(listenerType) || !listenerType.IsClass || listenerType.IsAbstract)
            {
                throw new ArgumentException($"the listener type must be implement IRabbitConsumerListener and none abstract class", "listenerType");
            }

            builder.Services.AddTransient(listenerType);
            return builder.AddListener(queue, (serviceProvider, result) =>
            {
                var listenner = serviceProvider.GetRequiredService(listenerType) as IRabbitConsumerListener;
                listenner.ConsumeAsync(result).Wait();
            });
        }
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddListener<T>(this IRabbitConsumerBuilder builder, string queue) where T : class, IRabbitConsumerListener
        {
            return builder.AddListener(queue, typeof(T));
        }
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="listenerType"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddListener(this IRabbitConsumerBuilder builder, string exchange, string queue, Type listenerType)
        {
            if (!typeof(IRabbitConsumerListener).IsAssignableFrom(listenerType) || !listenerType.IsClass || listenerType.IsAbstract)
            {
                throw new ArgumentException($"the listener type must be implement IRabbitConsumerListener and none abstract class", "listenerType");
            }

            builder.Services.AddTransient(listenerType);
            return builder.AddListener(exchange, queue, (serviceProvider, result) =>
            {
                var listenner = serviceProvider.GetService(listenerType) as IRabbitConsumerListener;
                listenner.ConsumeAsync(result).Wait();
            });
        }
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static IRabbitConsumerBuilder AddListener<T>(this IRabbitConsumerBuilder builder, string exchange, string queue) where T : class, IRabbitConsumerListener
        {
            return builder.AddListener(exchange, queue, typeof(T));
        }

    }
}
