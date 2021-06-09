using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCore.Kafka.Integration
{
    public class KafkaSubscriber
    {
        public KafkaSubscriber(string topic, int? partition)
        {
            Topic = topic;
            Partition = partition;
        }
        public KafkaSubscriber(string topic) : this(topic, null) { }
        public KafkaSubscriber() : this(null) { }

        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 分区
        /// </summary>
        public int? Partition { get; set; }

        public static KafkaSubscriber[] From(params string[] topics)
        {
            return topics.Select(f => new KafkaSubscriber()
            {
                Partition = null,
                Topic = f
            }).ToArray();
        }
    }
}
