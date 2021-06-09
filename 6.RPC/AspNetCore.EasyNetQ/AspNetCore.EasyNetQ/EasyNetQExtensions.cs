using AspNetCore.EasyNetQ.Consumers;
using AspNetCore.EasyNetQ.Producers;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ
{
    public static class EasyNetQExtensions
    {
        /// <summary>
        /// 注册EasyNetBus总线
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyNetQProducer(this IServiceCollection services, Action<EasyNetQProducerOptions> configure)
        {
            return services.AddEasyNetQProducer(Options.DefaultName, configure);
        }
        /// <summary>
        /// 注册EasyNetBus总线
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyNetQProducer(this IServiceCollection services, string name, Action<EasyNetQProducerOptions> configure)
        {
            services.TryAddSingleton<IBusClientFactory, DefaultBusClientFactory>();
            services.Configure(name, configure);
            return services;
        }

        /// <summary>
        /// 创建总线对象
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IBusClient Create(this IBusClientFactory factory)
        {
            return factory.Create(Options.DefaultName);
        }
        /// <summary>
        /// 发布订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busClient"></param>
        /// <param name="message"></param>
        public static void Publish<T>(this IBusClient busClient, T message) where T : class
        {
            busClient.Publish(message, null);
        }
        /// <summary>
        /// 发布订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busClient"></param>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        public static void Publish<T>(this IBusClient busClient, T message, string topic) where T : class
        {
            busClient.PublishAsync(message, topic).Wait();
        }
        /// <summary>
        /// 发布订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busClient"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task PublishAsync<T>(this IBusClient busClient, T message) where T : class
        {
            await busClient.PublishAsync(message, null);
        }
        /// <summary>
        /// 发送请求消息
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="busClient"></param>
        /// <param name="request"></param>
        public static TResponse Request<TRequest, TResponse>(this IBusClient busClient, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            return busClient.RequestAsync<TRequest, TResponse>(request).GetAwaiter().GetResult();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busClient"></param>
        /// <param name="message"></param>
        public static void Send<T>(this IBusClient busClient, T message) where T : class
        {
            busClient.Send(null, message);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busClient"></param>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        public static void Send<T>(this IBusClient busClient, string queue, T message) where T : class
        {
            busClient.SendAsync(queue, message).Wait();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busClient"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task SendAsync<T>(this IBusClient busClient, T message) where T : class
        {
            await busClient.SendAsync(null, message);
        }

        /// <summary>
        /// 注册消费者服务对象
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyNetQConsumerCore(this IServiceCollection services)
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
        public static IServiceCollection AddEasyNetQConsumer(this IServiceCollection services, Type serviceType)
        {
            if (!typeof(IEasyNetQConsumerProvider).IsAssignableFrom(serviceType) || !serviceType.IsClass || serviceType.IsAbstract)
            {
                throw new InvalidOperationException($"serviceType must be implement {nameof(IEasyNetQConsumerProvider)} and not abstract class ");
            }

            services.AddEasyNetQConsumerCore();
            services.AddSingleton(serviceType);
            return services;
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyNetQConsumer<T>(this IServiceCollection services) where T : class, IEasyNetQConsumerProvider
        {
            return services.AddEasyNetQConsumer(typeof(T));
        }
        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddEasyNetQConsumer(this IServiceCollection services, Action<EasyNetQConsumerOptions> configure)
        {
            EasyNetQConsumerOptions easyNetQConsumerOptions = new EasyNetQConsumerOptions();
            configure?.Invoke(easyNetQConsumerOptions);
            return services.AddEasyNetQConsumer(easyNetQConsumerOptions);
        }
        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="easyNetQConsumerOptions"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddEasyNetQConsumer(this IServiceCollection services, EasyNetQConsumerOptions easyNetQConsumerOptions)
        {
            services.AddEasyNetQConsumerCore();
            return new EasyNetQConsumerBuilder(services, easyNetQConsumerOptions);
        }

        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T>(this IEasyNetQConsumerBuilder builder, Action<T> onMessage) where T : class
        {
            return builder.AddSubscriber("", onMessage);
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T>(this IEasyNetQConsumerBuilder builder, Action<IServiceProvider, T> onMessage) where T : class
        {
            return builder.AddSubscriber("", onMessage);
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T>(this IEasyNetQConsumerBuilder builder, string subscriptionId, Action<T> onMessage) where T : class
        {
            return builder.AddSubscriber<T>(subscriptionId, (_, t) => onMessage?.Invoke(t));
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T>(this IEasyNetQConsumerBuilder builder, string subscriptionId, Action<IServiceProvider, T> onMessage) where T : class
        {
            return builder.AddSubscriber<T>(subscriptionId, async (s, t) =>
            {
                onMessage?.Invoke(s, t);
                await Task.CompletedTask;
            });
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T>(this IEasyNetQConsumerBuilder builder, Func<T, Task> onMessage) where T : class
        {
            return builder.AddSubscriber("", onMessage);
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T>(this IEasyNetQConsumerBuilder builder, Func<IServiceProvider, T, Task> onMessage) where T : class
        {
            return builder.AddSubscriber("", onMessage);
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T>(this IEasyNetQConsumerBuilder builder, string subscriptionId, Func<T, Task> onMessage) where T : class
        {
            return builder.AddSubscriber<T>(subscriptionId, (_, t) => onMessage?.Invoke(t));
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSubscriber"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T, TSubscriber>(this IEasyNetQConsumerBuilder builder)
            where T : class
            where TSubscriber : class, IEasyNetQSubscriber<T>
        {
            return builder.AddSubscriber<T, TSubscriber>("");
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSubscriber"></typeparam>
        /// <param name="builder"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber<T, TSubscriber>(this IEasyNetQConsumerBuilder builder, string subscriptionId)
            where T : class
            where TSubscriber : class, IEasyNetQSubscriber<T>
        {
            builder.Services.AddTransient<TSubscriber>();
            builder.AddSubscriber<T>(subscriptionId, async (serviceProvider, t) =>
            {
                var subscriber = serviceProvider.GetRequiredService<TSubscriber>();
                await subscriber.SubscribeAsync(t);
            });
            return builder;
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subscriberType"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber(this IEasyNetQConsumerBuilder builder, Type subscriberType)
        {
            return builder.AddSubscriber("", subscriberType);
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="subscriberType"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddSubscriber(this IEasyNetQConsumerBuilder builder, string subscriptionId, Type subscriberType)
        {
            var @interface = subscriberType.GetInterfaces().FirstOrDefault(f => f.IsGenericType && f.GetGenericTypeDefinition() == typeof(IEasyNetQSubscriber<>));
            if (@interface == null)
            {
                throw new ArgumentException($"the subscriber type must be implement IEasyNetQSubscriber<T> and none abstract class", nameof(subscriberType));
            }

            var messageType = @interface.GetGenericArguments().First();
            var addSubscriber = typeof(EasyNetQExtensions)
                .GetMethods()
                .Where(f => string.Equals(f.Name, nameof(EasyNetQExtensions.AddSubscriber)) &&
                    f.IsGenericMethod &&
                    f.GetGenericArguments().Length == 2 &&
                    f.GetParameters().LastOrDefault()?.ParameterType == typeof(string))
                .FirstOrDefault();

            addSubscriber.MakeGenericMethod(messageType, subscriberType).Invoke(null, new object[] { builder, subscriptionId });
            return builder;
        }

        /// <summary>
        /// 添加响应消息处理事件
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="builder"></param>
        /// <param name="responder"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddResponder<TRequest, TResponse>(this IEasyNetQConsumerBuilder builder, Func<TRequest, TResponse> responder)
            where TRequest : class
            where TResponse : class
        {
            return builder.AddResponder<TRequest, TResponse>(async (_, t) =>
            {
                var result = responder?.Invoke(t);
                return await Task.FromResult(result);
            });
        }
        /// <summary>
        /// 添加响应消息处理事件
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TResponder"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddResponder<TRequest, TResponse, TResponder>(this IEasyNetQConsumerBuilder builder)
            where TRequest : class
            where TResponse : class
            where TResponder : class, IEasyNetQResponder<TRequest, TResponse>
        {
            builder.Services.AddTransient<TResponder>();
            return builder.AddResponder<TRequest, TResponse>(async (serviceProvicer, request) =>
            {
                var responder = serviceProvicer.GetRequiredService<TResponder>();
                return await responder?.RespondAsync(request);
            });
        }
        /// <summary>
        /// 添加响应消息处理事件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="responderType"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddResponder(this IEasyNetQConsumerBuilder builder, Type responderType)
        {
            var @interface = responderType.GetInterfaces().FirstOrDefault(f => f.IsGenericType && f.GetGenericTypeDefinition() == typeof(IEasyNetQResponder<,>));
            if (@interface == null)
            {
                throw new ArgumentException($"the responder type must be implement IEasyNetQResponder<TRequest,TResponse> and none abstract class", nameof(responderType));
            }
            var types = @interface.GetGenericArguments();
            var requestType = types.First();
            var response = types.Last();
            var addResponder = typeof(EasyNetQExtensions)
                .GetMethods()
                .Where(f => string.Equals(f.Name, nameof(EasyNetQExtensions.AddResponder)) && f.IsGenericMethod && f.GetGenericArguments().Length == 3)
                .FirstOrDefault();

            addResponder.MakeGenericMethod(requestType, response, responderType).Invoke(null, new object[] { builder });
            return builder;
        }

        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="queue"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T>(this IEasyNetQConsumerBuilder builder, string queue, Action<T> onMessage) where T : class
        {
            return builder.AddReceiver<T>(queue, (_, t) =>
            {
                onMessage?.Invoke(t);
            });
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T>(this IEasyNetQConsumerBuilder builder, Action<T> onMessage) where T : class
        {
            return builder.AddReceiver<T>("", onMessage);
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T>(this IEasyNetQConsumerBuilder builder, Action<IServiceProvider, T> onMessage) where T : class
        {
            return builder.AddReceiver("", onMessage);
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="queue"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T>(this IEasyNetQConsumerBuilder builder, string queue, Action<IServiceProvider, T> onMessage) where T : class
        {
            return builder.AddReceiver<T>(queue, async (s, t) =>
            {
                onMessage?.Invoke(s, t);
                await Task.CompletedTask;
            });
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="queue"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T>(this IEasyNetQConsumerBuilder builder, string queue, Func<T, Task> onMessage) where T : class
        {
            return builder.AddReceiver<T>(queue, async (_, t) =>
            {
                await onMessage?.Invoke(t);
            });
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T>(this IEasyNetQConsumerBuilder builder, Func<T, Task> onMessage) where T : class
        {
            return builder.AddReceiver<T>("", onMessage);
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T>(this IEasyNetQConsumerBuilder builder, Func<IServiceProvider, T, Task> onMessage) where T : class
        {
            return builder.AddReceiver("", onMessage);
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReceiver"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T, TReceiver>(this IEasyNetQConsumerBuilder builder)
            where T : class
            where TReceiver : class, IEasyNetQReceiver<T>
        {
            return builder.AddReceiver<T, TReceiver>("");
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReceiver"></typeparam>
        /// <param name="builder"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver<T, TReceiver>(this IEasyNetQConsumerBuilder builder, string queue)
            where T : class
            where TReceiver : class, IEasyNetQReceiver<T>
        {
            builder.Services.AddTransient<TReceiver>();
            return builder.AddReceiver<T>(queue,async (serviceProvicer, t) =>
            {
                var receiver = serviceProvicer.GetRequiredService<TReceiver>();
                await receiver.ReceiveAsync(t);
            });
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="receiverType"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver(this IEasyNetQConsumerBuilder builder, Type receiverType)
        {
            return builder.AddReceiver("", receiverType);
        }
        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="queue"></param>
        /// <param name="receiverType"></param>
        /// <returns></returns>
        public static IEasyNetQConsumerBuilder AddReceiver(this IEasyNetQConsumerBuilder builder, string queue, Type receiverType)
        {
            var @interface = receiverType.GetInterfaces().FirstOrDefault(f => f.IsGenericType && f.GetGenericTypeDefinition() == typeof(IEasyNetQReceiver<>));
            if (@interface == null)
            {
                throw new ArgumentException($"the receiver type must be implement IEasyNetQReceiver<> and none abstract class", "receiverType");
            }
            var messageType = @interface.GetGenericArguments().First();
            var addReceiver = typeof(EasyNetQExtensions)
                .GetMethods()
                .Where(f => string.Equals(f.Name, nameof(EasyNetQExtensions.AddReceiver)))
                .Where(f => f.IsGenericMethod && f.GetGenericArguments().Length == 2 && f.GetParameters().LastOrDefault()?.ParameterType == typeof(string))
                .FirstOrDefault();

            addReceiver.MakeGenericMethod(messageType, receiverType).Invoke(null, new object[] { builder, queue });
            return builder;
        }

    }
}
