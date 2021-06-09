using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ.Consumers
{
    public class EasyNetQConsumerBuilder : IEasyNetQConsumerBuilder
    {
        EasyNetQConsumerOptions easyNetQConsumerOptions;
        Guid guid = Guid.NewGuid();
        bool receiveConsumerProviderIsRegister = false;

        public EasyNetQConsumerBuilder(IServiceCollection services, EasyNetQConsumerOptions easyNetQConsumerOptions)
        {
            this.Services = services;
            this.easyNetQConsumerOptions = easyNetQConsumerOptions;
        }

        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// 添加接收消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public IEasyNetQConsumerBuilder AddReceiver<T>(string queue, Func<IServiceProvider, T, Task> onMessage) where T : class
        {
            queue = string.IsNullOrEmpty(queue) ? easyNetQConsumerOptions.Queue : queue;

            if (string.IsNullOrEmpty(queue))
            {
                throw new ArgumentException($"queue cann't be empty", nameof(queue));
            }

            if (!receiveConsumerProviderIsRegister)
            {
                receiveConsumerProviderIsRegister = true;
                Services.AddSingleton<IEasyNetQConsumerProvider>(serviceProvider =>
                {
                    var handlers = serviceProvider.GetService<IEnumerable<IReceiveHandler>>();
                    return new ReceiveConsumerProvider(easyNetQConsumerOptions, handlers.Where(f => f.Id == guid).ToArray());
                });
            }
            Services.AddSingleton<IReceiveHandler>(serviceProvider =>
            {
                return new EasyNetQReceiveHandler<T>(guid, queue, async t =>
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        await onMessage?.Invoke(scope.ServiceProvider, t);
                    }
                });
            });
            return this;
        }
        /// <summary>
        /// 添加响应消息处理事件
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="responder"></param>
        /// <returns></returns>
        public IEasyNetQConsumerBuilder AddResponder<TRequest, TResponse>(Func<IServiceProvider, TRequest, Task<TResponse>> responder)
            where TRequest : class
            where TResponse : class
        {
            Services.AddSingleton<IEasyNetQConsumerProvider>(serviceProvider =>
            {
                return new RespondConsumerProvider<TRequest, TResponse>(easyNetQConsumerOptions, async request =>
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        return await responder.Invoke(scope.ServiceProvider, request);
                    }
                 });
            });

            return this;
        }
        /// <summary>
        /// 添加订阅消息处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriptionId"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public IEasyNetQConsumerBuilder AddSubscriber<T>(string subscriptionId, Func<IServiceProvider, T, Task> onMessage) where T : class
        {
            Services.AddSingleton<IEasyNetQConsumerProvider>(serviceProvider =>
            {
                return new SubscribeConsumerProvider<T>(easyNetQConsumerOptions, subscriptionId, async result =>
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        await onMessage?.Invoke(scope.ServiceProvider, result);
                    }
                });
            });

            return this;
        }

    }
}
