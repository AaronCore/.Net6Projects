using AspNetCore.RabbitMQ.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ
{
    public abstract class RabbitOptions : RabbitBaseOptions
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
    }
}
