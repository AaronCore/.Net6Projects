using AspNetCore.Kafka.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Kafka
{
    public class KafkaConsumerOptions : KafkaBaseOptions
    {
        /// <summary>
        /// 群组
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 订阅主题
        /// </summary>
        public KafkaSubscriber[] Subscribers { get; set; }
        /// <summary>
        /// 是否允许自动提交（enable.auto.commit）
        /// </summary>
        public bool EnableAutoCommit { get; set; } = false;
    }
}
