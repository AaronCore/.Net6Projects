using AspNetCore.Kafka.Consumers;
using AspNetCore.Kafka.Integration;
using AspNetCore.Kafka.Producers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Kafka
{
    public static class KafkaExtensions
    {
        /// <summary>
        /// 注册添加一个发送者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaProducer(this IServiceCollection services, Action<KafkaProducerOptions> configure)
        {
            return services.AddKafkaProducer(Options.DefaultName, configure);
        }
        /// <summary>
        /// 注册添加一个发送者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaProducer(this IServiceCollection services, string name, Action<KafkaProducerOptions> configure)
        {
            services.TryAddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
            services.Configure(name, configure);
            return services;
        }

        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaConsumerCore(this IServiceCollection services)
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
        public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, Type serviceType)
        {
            if (!typeof(IKafkaConsumerProvider).IsAssignableFrom(serviceType) || !serviceType.IsClass || serviceType.IsAbstract)
            {
                throw new InvalidOperationException($"serviceType must be implement {nameof(IKafkaConsumerProvider)} and not abstract class ");
            }

            services.AddKafkaConsumerCore();
            services.AddSingleton(serviceType);
            return services;
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaConsumer<T>(this IServiceCollection services) where T : class, IKafkaConsumerProvider
        {
            return services.AddKafkaConsumer(typeof(T));
        }
        /// <summary>
        /// 注册一个消费者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="kafkaConsumerProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IKafkaConsumerProvider kafkaConsumerProvider)
        {
            services.AddKafkaConsumerCore();
            services.AddSingleton(kafkaConsumerProvider);
            return services;
        }

        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IKafkaConsumerBuilder AddKafkaConsumer(this IServiceCollection services, Action<KafkaConsumerOptions> configure)
        {
            KafkaConsumerOptions kafkaConsumerOptions = new KafkaConsumerOptions();
            configure?.Invoke(kafkaConsumerOptions);
            return services.AddKafkaConsumer(kafkaConsumerOptions);
        }
        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="kafkaConsumerOptions"></param>
        /// <returns></returns>
        public static IKafkaConsumerBuilder AddKafkaConsumer(this IServiceCollection services, KafkaConsumerOptions kafkaConsumerOptions)
        {
            services.AddKafkaConsumerCore();
            return new KafkaConsumerBuilder(services, kafkaConsumerOptions);
        }
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public static IKafkaConsumerBuilder AddListener(this IKafkaConsumerBuilder builder, Action<RecieveResult> onMessageRecieved)
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
        public static IKafkaConsumerBuilder AddListener(this IKafkaConsumerBuilder builder, Type listenerType)
        {
            if (!typeof(IKafkaConsumerListener).IsAssignableFrom(listenerType) || !listenerType.IsClass || listenerType.IsAbstract)
            {
                throw new ArgumentException($"the listener type must be implement {nameof(IKafkaConsumerListener)} and none abstract class", nameof(listenerType));
            }

            builder.Services.AddTransient(listenerType);
            return builder.AddListener((serviceProvider, result) =>
            {
                var listenner = serviceProvider.GetService(listenerType) as IKafkaConsumerListener;
                listenner.ConsumeAsync(result).Wait();
            });
        }
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IKafkaConsumerBuilder AddListener<T>(this IKafkaConsumerBuilder builder) where T : class, IKafkaConsumerListener
        {
            return builder.AddListener(typeof(T));
        }

        /// <summary>
        /// 创建客户端生产者
        /// </summary>
        /// <param name="kafkaProducerFactory"></param>
        /// <returns></returns>
        public static IKafkaClientProducer Create(this IKafkaProducerFactory kafkaProducerFactory)
        {
            return kafkaProducerFactory.Create(Options.DefaultName);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public static void Publish(this IKafkaClientProducer producer, object message, Action<DeliveryResult> callback = null)
        {
            if (message is KafkaMessage kafkaMessage)
            {
                producer.Publish(kafkaMessage, callback);
            }
            else
            {
                producer.Publish(null, message, callback);
            }
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public static void Publish(this IKafkaClientProducer producer, string key, object message, Action<DeliveryResult> callback = null)
        {
            producer.Publish(null, key, message, callback);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="partition"></param>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public static void Publish(this IKafkaClientProducer producer, int? partition, string key, object message, Action<DeliveryResult> callback = null)
        {
            producer.Publish(new KafkaMessage()
            {
                Key = key,
                Message = message,
                Partition = partition
            }, callback);
        }

        /// <summary>
        /// 异步发布消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="message"></param>
        public static async Task<DeliveryResult> PublishAsync(this IKafkaClientProducer producer, object message)
        {
            if (message is KafkaMessage kafkaMessage)
            {
                return await producer.PublishAsync(kafkaMessage);
            }
            else
            {
                return await producer.PublishAsync(null, message);
            }
        } /// <summary>
          /// 异步发布消息
          /// </summary>
          /// <param name="producer"></param>
          /// <param name="key"></param>
          /// <param name="message"></param>
        public static async Task<DeliveryResult> PublishAsync(this IKafkaClientProducer producer, string key, object message)
        {
            return await producer.PublishAsync(null, key, message);
        }
        /// <summary>
        /// 异步发布消息
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="partition"></param>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public static async Task<DeliveryResult> PublishAsync(this IKafkaClientProducer producer, int? partition, string key, object message)
        {
            return await producer.PublishAsync(new KafkaMessage()
            {
                Key = key,
                Message = message,
                Partition = partition
            });
        }
    }
}
