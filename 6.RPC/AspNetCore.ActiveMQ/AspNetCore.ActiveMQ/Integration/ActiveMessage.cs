using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ActiveMQ.Integration
{
    public class ActiveMessage
    {
        /// <summary>
        /// 队列或者Topic
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 是否持久化消息
        /// </summary>
        public bool? IsPersistent { get; set; } = true;
        /// <summary>
        /// 消息的有效时间
        /// </summary>
        public TimeSpan? TimeToLive { get; set; } = null;
        /// <summary>
        /// 消息属性
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
}
