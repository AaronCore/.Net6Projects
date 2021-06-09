using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using EasyNetQ.Consumer;

namespace AspNetCore.EasyNetQ.Consumers
{
    public class ReceiveConsumerProvider : IEasyNetQConsumerProvider
    {
        IBus bus;
        EasyNetQConsumerOptions easyNetQConsumerOptions;
        IEnumerable<IReceiveHandler> handlers;
        List<IDisposable> results;
        bool disposed = false;
        string queue;

        public ReceiveConsumerProvider(EasyNetQConsumerOptions easyNetQConsumerOptions, IEnumerable<IReceiveHandler> handlers)
        {
            this.handlers = handlers;
            this.easyNetQConsumerOptions = easyNetQConsumerOptions;
            this.bus = easyNetQConsumerOptions.CreateBus();
        }

        private async Task<IDisposable> ListenAsync(string queue, IEnumerable<IReceiveHandler> handlers)
        {
            var result = await bus.SendReceive.ReceiveAsync(queue, async registration =>
            {
                foreach (var handler in handlers)
                {
                    await handler.ResolveAsync(registration);
                }
            }, cfg =>
            {
                if (easyNetQConsumerOptions.Priority != null)
                {
                    cfg.WithPriority(easyNetQConsumerOptions.Priority.Value);
                }
                if (easyNetQConsumerOptions.Exclusive != null)
                {
                    cfg.WithExclusive(easyNetQConsumerOptions.Exclusive.Value);
                }
                if (easyNetQConsumerOptions.PrefetchCount > 0)
                {
                    cfg.WithPrefetchCount(easyNetQConsumerOptions.PrefetchCount);
                }
                if (easyNetQConsumerOptions != null)
                {
                    foreach (var item in easyNetQConsumerOptions.Arguments)
                    {
                        cfg.WithArgument(item.Key, item.Value);
                    }
                }
            });
            return result;
        }

        public async Task ListenAsync()
        {
            CheckDisposed();

            if (results == null)
            {
                results = new List<IDisposable>();
                queue = string.Join(",", handlers.Select(f => f.Queue));
                foreach (var g in handlers.GroupBy(f => f.Queue))
                {
                    var result = await ListenAsync(g.Key, g);
                    results.Add(result);
                }
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        result.Dispose();
                    }
                }
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
            return $"queue:{queue},IsConnected-{bus?.Advanced?.IsConnected ?? false}";
        }
    }

}
