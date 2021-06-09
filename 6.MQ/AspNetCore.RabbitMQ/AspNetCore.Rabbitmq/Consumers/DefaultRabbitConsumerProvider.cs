using AspNetCore.RabbitMQ.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.RabbitMQ.Consumers
{
    public class DefaultRabbitConsumerProvider : IRabbitConsumerProvider
    {
        RabbitConsumerOptions rabbitConsumerOptions;
        string exchange;
        string queue;
        ListenResult listenResult;
        bool disposed = false;
        Action<RecieveResult> action;

        public DefaultRabbitConsumerProvider(string queue, RabbitConsumerOptions rabbitConsumerOptions, Action<RecieveResult> action) : this("", queue, rabbitConsumerOptions, action) { }
        public DefaultRabbitConsumerProvider(string exchange, string queue, RabbitConsumerOptions rabbitConsumerOptions, Action<RecieveResult> action)
        {
            this.rabbitConsumerOptions = rabbitConsumerOptions;
            this.exchange = exchange;
            this.queue = queue;
            this.action = action;

            Consumer = RabbitConsumer.Create(rabbitConsumerOptions);
        }

        public RabbitConsumer Consumer { get; }

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
            if (disposed) throw new ObjectDisposedException(nameof(DefaultRabbitConsumerProvider));

            if (listenResult == null)
            {
                if (string.IsNullOrEmpty(exchange))
                {
                    listenResult = Consumer.Listen(queue, options =>
                    {
                        options.AutoDelete = rabbitConsumerOptions.AutoDelete;
                        options.Durable = rabbitConsumerOptions.Durable;
                        options.AutoAck = rabbitConsumerOptions.AutoAck;
                        options.Arguments = rabbitConsumerOptions.Arguments;
                        options.FetchCount = rabbitConsumerOptions.FetchCount;
                    }, action);
                }
                else
                {
                    listenResult = Consumer.Listen(exchange, queue, options =>
                    {
                        options.AutoDelete = rabbitConsumerOptions.AutoDelete;
                        options.Durable = rabbitConsumerOptions.Durable;
                        options.AutoAck = rabbitConsumerOptions.AutoAck;
                        options.Arguments = rabbitConsumerOptions.Arguments;
                        options.FetchCount = rabbitConsumerOptions.FetchCount;
                        options.Type = rabbitConsumerOptions.Type;
                        options.RouteQueues = rabbitConsumerOptions.RouteQueues;
                    }, action);
                }
            }

            await Task.CompletedTask;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(exchange))
            {
                return $"queue:{queue} from {Consumer}";
            }
            else
            {
                return $"queue:{queue}(exchange:{exchange}) from {Consumer}";
            }
        }
    }
}
