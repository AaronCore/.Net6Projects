using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.EasyNetQ
{
    public class EasyNetQConsumerOptions: EasyNetQOptions
    {
        /// <summary>
        /// 消息队列
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool? AutoDelete { get; set; }
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool? Durable { get; set; }
        /// <summary>
        /// 是否专属通道
        /// </summary>
        public bool? Exclusive { get; set; }
        /// <summary>
        /// 优先等级
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// Topic   
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}
