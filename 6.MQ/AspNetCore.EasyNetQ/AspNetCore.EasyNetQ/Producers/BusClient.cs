using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ.Producers
{
    public class BusClient : IBusClient
    {
        EasyNetQProducerOptions easyNetQProducerOptions;
        IBus bus;
        bool disposed = false;

        public BusClient(EasyNetQProducerOptions easyNetQProducerOptions)
        {
            this.easyNetQProducerOptions = easyNetQProducerOptions;
            this.bus = easyNetQProducerOptions.CreateBus();
        }

        public bool IsConnected { get { return bus?.Advanced?.IsConnected ?? false; } }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                bus?.Dispose();
            }
        }

        private void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// 发布订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task PublishAsync<T>(T message, string topic) where T : class
        {
            CheckDisposed();

            topic = string.IsNullOrEmpty(topic) ? easyNetQProducerOptions.Topic : topic;
            await bus.PubSub.PublishAsync(message, cfg =>
            {
                if (!string.IsNullOrEmpty(topic))
                {
                    cfg.WithTopic(topic);
                }
                if (easyNetQProducerOptions.Priority != null)
                {
                    cfg.WithPriority(easyNetQProducerOptions.Priority.Value);
                }
            });
        }
        /// <summary>
        /// 发送请求消息
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class
        {
            CheckDisposed();
            return await bus.Rpc.RequestAsync<TRequest, TResponse>(request, cfg =>
            {
                if (easyNetQProducerOptions.Priority != null)
                {
                    cfg.WithPriority(easyNetQProducerOptions.Priority.Value);
                }
                if (!string.IsNullOrEmpty(easyNetQProducerOptions.Queue))
                {
                    cfg.WithQueueName(easyNetQProducerOptions.Queue);
                }
            });
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync<T>(string queue, T message) where T : class
        {
            CheckDisposed();
            queue = string.IsNullOrEmpty(queue) ? easyNetQProducerOptions.Queue : queue;
            await bus.SendReceive.SendAsync(queue, message, cfg =>
            {
                if (easyNetQProducerOptions.Priority != null)
                {
                    cfg.WithPriority(easyNetQProducerOptions.Priority.Value);
                }
            });
        }
    }
}
