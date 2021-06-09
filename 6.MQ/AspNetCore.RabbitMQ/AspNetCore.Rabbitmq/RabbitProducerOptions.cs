using AspNetCore.RabbitMQ.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ
{
    public class RabbitProducerOptions : RabbitOptions
    {
        /// <summary>
        /// 保留发布者数
        /// </summary>
        public int InitializeCount { get; set; } = 5;
        /// <summary>
        /// 队列
        /// </summary>
        public string[] Queues { get; set; }
        /// <summary>
        /// 交换机
        /// </summary>
        public string Exchange { get; set; }
        /// <summary>
        /// 交换机类型
        /// </summary>
        public RabbitExchangeType Type { get; set; }
        /// <summary>
        /// 队列及路由值
        /// </summary>
        public RouteQueue[] RouteQueues { get; set; }
    }
}
