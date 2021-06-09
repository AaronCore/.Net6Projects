using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.RabbitMQ
{
    public interface IRabbitClientProducer : IDisposable
    {
        /// <summary>
        /// 普通的往队列发送消息
        /// </summary>
        /// <param name="messages"></param>
        void Publish(string[] messages);
        /// <summary>
        /// 使用交换机发送消息
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="messages"></param>
        void Publish(string routingKey, string[] messages);
    }
}
