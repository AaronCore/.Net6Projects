using System;
using System.Threading.Tasks;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ.Consumers
{
    public class DefaultActiveConsumerProvider : IActiveConsumerProvider
    {
        ActiveConsumerOptions activeConsumerOptions;
        Action<RecieveResult> action;
        ListenResult listenResult;
        bool disposed = false;

        public DefaultActiveConsumerProvider(ActiveConsumerOptions activeConsumerOptions, Action<RecieveResult> action)
        {
            this.action = action;
            this.activeConsumerOptions = activeConsumerOptions;

            Consumer = ActiveConsumer.Create(activeConsumerOptions);
        }

        public ActiveConsumer Consumer { get; }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                listenResult?.Stop();
                Consumer?.Dispose();
            }
        }

        public async Task ListenAsync()
        {
            if (disposed) throw new ObjectDisposedException(nameof(DefaultActiveConsumerProvider));

            if (listenResult == null)
            {
                var listenOptions = new ListenOptions()
                {
                    FromQueue = activeConsumerOptions.FromQueue,
                    AutoAcknowledge = activeConsumerOptions.AutoAcknowledge,
                    ClientId = activeConsumerOptions.ClientId,
                    Destination = activeConsumerOptions.Destination,
                    Durable = activeConsumerOptions.Durable,
                    Interval = activeConsumerOptions.Interval,
                    PrefetchCount = activeConsumerOptions.PrefetchCount,
                    RecoverWhenNotAcknowledge = activeConsumerOptions.RecoverWhenNotAcknowledge,
                    Selector = activeConsumerOptions.Selector,
                    SubscriberName = activeConsumerOptions.SubscriberName
                };
                listenResult = await Consumer.ListenAsync(listenOptions, action);
            }
        }
        public override string ToString()
        {
            return Consumer?.ToString();
        }
    }
}
