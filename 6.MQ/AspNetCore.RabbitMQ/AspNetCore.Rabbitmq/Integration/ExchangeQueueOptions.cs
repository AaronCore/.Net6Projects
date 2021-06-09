using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Integration
{

    public class ExchangeQueueOptions
    {
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; } = true;
        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool AutoDelete { get; set; } = false;
        /// <summary>
        /// 参数,例如：{ "x-queue-type", "classic" }
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
        /// <summary>
        /// 交换机类型
        /// </summary>
        public RabbitExchangeType Type { get; set; }
        /// <summary>
        /// 队列及路由值
        /// </summary>
        public RouteQueue[] RouteQueues { get; set; }
    }
    public class ExchangeConsumeQueueOptions : ExchangeQueueOptions
    {
        /// <summary>
        /// 是否自动提交
        /// </summary>
        public bool AutoAck { get; set; } = false;
        /// <summary>
        /// 每次发送消息条数
        /// </summary>
        public ushort? FetchCount { get; set; }
    }
}
