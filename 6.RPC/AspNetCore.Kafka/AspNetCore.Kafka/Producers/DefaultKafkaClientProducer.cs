using AspNetCore.Kafka.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Kafka.Producers
{

    public class DefaultKafkaClientProducer : IKafkaClientProducer
    {
        KafkaProducerOptions kafkaProducerOptions;
        KafkaProducer producer;

        public DefaultKafkaClientProducer(KafkaProducerOptions kafkaProducerOptions)
        {
            this.kafkaProducerOptions = kafkaProducerOptions;
        }

        public bool Disposed { get; private set; }

        private KafkaProducer GetProducer()
        {
            if (producer == null)
            {
                lock (this)
                {
                    if (producer == null)
                    {
                        producer = KafkaProducer.Create(kafkaProducerOptions);
                        producer.DefaultKey = kafkaProducerOptions.Key;
                        producer.DefaultTopic = kafkaProducerOptions.Topic;
                        producer.InitializeCount = kafkaProducerOptions.InitializeCount;
                    }
                }
            }
            return producer;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                producer?.Dispose();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="kafkaMessage"></param>
        /// <param name="callback"></param>
        public void Publish(KafkaMessage kafkaMessage, Action<DeliveryResult> callback = null)
        {
            var producer = GetProducer();

            producer.Publish(new KafkaMessage()
            {
                Key = string.IsNullOrEmpty(kafkaMessage.Key) ? kafkaProducerOptions.Key : kafkaMessage.Key,
                Message = kafkaMessage.Message,
                Partition = kafkaMessage.Partition ?? kafkaProducerOptions.Partition,
                Topic = string.IsNullOrEmpty(kafkaMessage.Topic) ? kafkaProducerOptions.Topic : kafkaMessage.Topic
            }, callback);
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="kafkaMessage"></param>
        /// <returns></returns>
        public async Task<DeliveryResult> PublishAsync(KafkaMessage kafkaMessage)
        {
            var producer = GetProducer();
            return await producer.PublishAsync(new KafkaMessage()
            {
                Key = string.IsNullOrEmpty(kafkaMessage.Key) ? kafkaProducerOptions.Key : kafkaMessage.Key,
                Message = kafkaMessage.Message,
                Partition = kafkaMessage.Partition ?? kafkaProducerOptions.Partition,
                Topic = string.IsNullOrEmpty(kafkaMessage.Topic) ? kafkaProducerOptions.Topic : kafkaMessage.Topic
            });
        }
    }
}
