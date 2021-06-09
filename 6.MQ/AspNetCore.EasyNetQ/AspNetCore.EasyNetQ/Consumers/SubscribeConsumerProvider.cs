using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ.Consumers
{
    public class SubscribeConsumerProvider<T> : IEasyNetQConsumerProvider where T : class
    {
        string subscriptionId;
        IBus bus;
        EasyNetQConsumerOptions easyNetQConsumerOptions;
        IDisposable result;
        bool disposed = false;
        Func<T, Task> onMessageRecieved;

        public SubscribeConsumerProvider(EasyNetQConsumerOptions easyNetQConsumerOptions, string subscriptionId, Func<T, Task> onMessageRecieved)
        {
            this.easyNetQConsumerOptions = easyNetQConsumerOptions;
            this.subscriptionId = subscriptionId;
            this.onMessageRecieved = onMessageRecieved;
            this.bus = easyNetQConsumerOptions.CreateBus();
        }

        public async Task ListenAsync()
        {
            CheckDisposed();

            if (result == null)
            {
                result = await bus.PubSub.SubscribeAsync<T>(subscriptionId, async t =>
                {
                    await onMessageRecieved?.Invoke(t);
                }, cfg =>
                {
                    if (!string.IsNullOrEmpty(easyNetQConsumerOptions.Queue))
                    {
                        cfg.WithQueueName(easyNetQConsumerOptions.Queue);
                    }
                    if (easyNetQConsumerOptions.AutoDelete != null)
                    {
                        cfg.WithAutoDelete(easyNetQConsumerOptions.AutoDelete.Value);
                    }
                    if (easyNetQConsumerOptions.Durable != null)
                    {
                        cfg.WithDurable(easyNetQConsumerOptions.Durable.Value);
                    }
                    if (easyNetQConsumerOptions.Priority != null)
                    {
                        cfg.WithPriority(easyNetQConsumerOptions.Priority.Value);
                    }
                    if (!string.IsNullOrEmpty(easyNetQConsumerOptions.Topic))
                    {
                        cfg.WithTopic(easyNetQConsumerOptions.Topic);
                    }
                    if (easyNetQConsumerOptions.PrefetchCount > 0)
                    {
                        cfg.WithPrefetchCount(easyNetQConsumerOptions.PrefetchCount);
                    }
                });
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                result?.Dispose();
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

        public override string ToString()
        {
            return $"subscriptionId-{subscriptionId},IsConnected-{bus?.Advanced?.IsConnected ?? false}";
        }
    }
}
