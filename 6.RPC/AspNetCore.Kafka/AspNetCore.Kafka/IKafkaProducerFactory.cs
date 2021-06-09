using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Kafka
{
    public interface IKafkaProducerFactory
    {
        /// <summary>
        /// 创建客户端生产者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IKafkaClientProducer Create(string name);
    }
}
