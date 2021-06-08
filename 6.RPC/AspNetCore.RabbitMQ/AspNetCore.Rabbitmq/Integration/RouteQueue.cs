using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Integration
{
    public class RouteQueue
    {
        /// <summary>
        /// 路由
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// 队列
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// 队列选项
        /// </summary>
        public QueueOptions Options { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}
