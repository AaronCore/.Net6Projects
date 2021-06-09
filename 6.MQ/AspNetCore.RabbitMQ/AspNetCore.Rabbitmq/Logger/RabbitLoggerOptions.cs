using AspNetCore.RabbitMQ.Integration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Logger
{
    public class RabbitLoggerOptions: RabbitOptions
    {
        /// <summary>
        /// 队列
        /// </summary>
        public string Queue { get; set; }
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
        /// <summary>
        /// 最低日志记录
        /// </summary>
        public LogLevel MinLevel { get; set; } = LogLevel.Information;
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; } = "";
        /// <summary>
        /// 应用名称
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// 保留发布者数
        /// </summary>
        public int InitializeCount { get; set; } = 10;
    }
}
