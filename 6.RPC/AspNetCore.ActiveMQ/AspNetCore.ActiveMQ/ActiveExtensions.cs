using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AspNetCore.Activemq.Consumers;
using AspNetCore.ActiveMQ.Consumers;
using AspNetCore.ActiveMQ.Integration;
using AspNetCore.Activemq.Producers;

namespace AspNetCore.ActiveMQ
{
    public static class ActiveExtensions
    {
        /// <summary>
        /// 注册添加一个发送者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddActiveProducer(this IServiceCollection services, Action<ActiveProducerOptions> configure)
        {
            return services.AddActiveProducer(Options.DefaultName, configure);
        }
        /// <summary>
        /// 注册添加一个发送者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddActiveProducer(this IServiceCollection services, string name, Action<ActiveProducerOptions> configure)
        {
            services.TryAddSingleton<IActiveProducerFactory, DefaultActiveProducerFactory>();
            services.Configure(name, configure);
            return services;
        }

        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddActiveConsumerCore(this IServiceCollection services)
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
        public static IServiceCollection AddActiveConsumer(this IServiceCollection services, Type serviceType)
        {
            if (!typeof(IActiveConsumerProvider).IsAssignableFrom(serviceType) || !serviceType.IsClass || serviceType.IsAbstract)
            {
                throw new InvalidOperationException($"serviceType must be implement {nameof(IActiveConsumerProvider)} and not abstract class ");
            }

            services.AddActiveConsumerCore();
            services.AddSingleton(serviceType);
            return services;
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddActiveConsumer<T>(this IServiceCollection services) where T : class, IActiveConsumerProvider
        {
            return services.AddActiveConsumer(typeof(T));
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="activeConsumerProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddActiveConsumer(this IServiceCollection services, IActiveConsumerProvider activeConsumerProvider)
        {
            services.AddActiveConsumerCore();
            services.AddSingleton(activeConsumerProvider);
            return services;
        }

        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IActiveConsumerBuilder AddActiveConsumer(this IServiceCollection services, Action<ActiveConsumerOptions> configure)
        {
            ActiveConsumerOptions activeConsumerOptions = new ActiveConsumerOptions();
            configure?.Invoke(activeConsumerOptions);
            return services.AddActiveConsumer(activeConsumerOptions);
        }
        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="activeConsumerOptions"></param>
        /// <returns></returns>
        public static IActiveConsumerBuilder AddActiveConsumer(this IServiceCollection services, ActiveConsumerOptions activeConsumerOptions)
        {
            services.AddActiveConsumerCore();
            return new ActiveConsumerBuilder(services, activeConsumerOptions);
        }
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public static IActiveConsumerBuilder AddListener(this IActiveConsumerBuilder builder, Action<RecieveResult> onMessageRecieved)
        {
            return builder.AddListener((_, result) =>
            {
                onMessageRecieved?.Invoke(result);
            });
        }
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="listenerType"></param>
        /// <returns></returns>
        public static IActiveConsumerBuilder AddListener(this IActiveConsumerBuilder builder, Type listenerType)
        {
            if (!typeof(IActiveConsumerListener).IsAssignableFrom(listenerType) || !listenerType.IsClass || listenerType.IsAbstract)
            {
                throw new ArgumentException($"the listener type must be implement {nameof(IActiveConsumerListener)} and none abstract class", nameof(listenerType));
            }

            builder.Services.AddTransient(listenerType);
            return builder.AddListener((serviceProvider, result) =>
            {
                var listenner = serviceProvider.GetService(listenerType) as IActiveConsumerListener;
                listenner.ConsumeAsync(result).Wait();
            });
        }
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IActiveConsumerBuilder AddListener<T>(this IActiveConsumerBuilder builder) where T : class, IActiveConsumerListener
        {
            return builder.AddListener(typeof(T));
        }

        /// <summary>
        /// 创建客户端生产者
        /// </summary>
        /// <param name="activeProducerFactory"></param>
        /// <returns></returns>
        public static IActiveClientProducer Create(this IActiveProducerFactory activeProducerFactory)
        {
            return activeProducerFactory.Create(Options.DefaultName);
        }

        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="activeClientProducer"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static async Task SendAsync(this IActiveClientProducer activeClientProducer, params string[] messages)
        {
            await activeClientProducer.SendAsync(messages.Select(f => new ActiveMessage()
            {
                Message = f
            }).ToArray());
        }
        /// <summary>
        /// 发送消息到Topic
        /// </summary>
        /// <param name="activeClientProducer"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static async Task PublishAsync(this IActiveClientProducer activeClientProducer, params string[] messages)
        {
            await activeClientProducer.PublishAsync(messages.Select(f => new ActiveMessage()
            {
                Message = f
            }).ToArray());
        }
    }
}
