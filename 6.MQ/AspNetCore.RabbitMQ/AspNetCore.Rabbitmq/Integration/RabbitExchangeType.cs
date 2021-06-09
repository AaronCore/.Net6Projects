using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Integration
{
    public enum RabbitExchangeType
    {
        /// <summary>
        /// 普通模式
        /// </summary>
        None = 0,
        /// <summary>
        /// 路由模式
        /// </summary>
        Direct = 1,
        /// <summary>
        /// 发布/订阅模式
        /// </summary>
        Fanout = 2,
        /// <summary>
        /// 匹配订阅模式
        /// </summary>
        Topic = 3
    }
}
