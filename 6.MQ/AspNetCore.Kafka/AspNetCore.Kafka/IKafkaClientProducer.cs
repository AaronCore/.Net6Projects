using AspNetCore.Kafka.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Kafka
{
    public interface IKafkaClientProducer : IDisposable
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="kafkaMessage"></param>
        /// <param name="callback"></param>
        void Publish(KafkaMessage kafkaMessage, Action<DeliveryResult> callback = null);
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="kafkaMessage"></param>
        /// <returns></returns>
        Task<DeliveryResult> PublishAsync(KafkaMessage kafkaMessage);
    }
}
