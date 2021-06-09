using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AspNetCore.RabbitMQ.Integration;

namespace AspNetCore.RabbitMQ
{
    public class RabbitClientProducer : BaseProducerPool, IRabbitClientProducer
    {
        RabbitProducerOptions rabbitProducerOptions;
        public RabbitClientProducer(RabbitProducerOptions rabbitProducerOptions) : base(rabbitProducerOptions)
        {
            this.rabbitProducerOptions = rabbitProducerOptions;
        }

        protected override int InitializeCount => rabbitProducerOptions.InitializeCount;

        /// <summary>
        /// 普通的往队列发送消息
        /// </summary>
        /// <param name="messages"></param>
        public void Publish(string[] messages)
        {
            var producer = RentProducer();

            foreach (var queue in rabbitProducerOptions.Queues)
            {
                if (!string.IsNullOrEmpty(queue))
                {
                    producer.Publish(queue, messages, new QueueOptions()
                    {
                        Arguments = rabbitProducerOptions.Arguments,
                        AutoDelete = rabbitProducerOptions.AutoDelete,
                        Durable = rabbitProducerOptions.Durable
                    });
                }
            }

            ReturnProducer(producer);
        }
        /// <summary>
        /// 使用交换机发送消息
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="messages"></param>
        public void Publish(string routingKey, string[] messages)
        {
            var producer = RentProducer();

            producer.Publish(rabbitProducerOptions.Exchange, routingKey, messages, new ExchangeQueueOptions()
            {
                Arguments = rabbitProducerOptions.Arguments,
                AutoDelete = rabbitProducerOptions.AutoDelete,
                Durable = rabbitProducerOptions.Durable,
                RouteQueues = rabbitProducerOptions.RouteQueues,
                Type = rabbitProducerOptions.Type
            });

            ReturnProducer(producer);
        }

    }
}
