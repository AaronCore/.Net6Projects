using AspNetCore.RabbitMQ.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ
{
    public class RabbitConsumerOptions: RabbitOptions
    {
        /// <summary>
        /// 是否自动提交
        /// </summary>
        public bool AutoAck { get; set; } = false;
        /// <summary>
        /// 每次发送消息条数
        /// </summary>
        public ushort? FetchCount { get; set; }
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
