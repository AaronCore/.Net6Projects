using System;
using System.Collections.Generic;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ
{
    public class ActiveProducerOptions : ActiveBaseOptions
    {
        /// <summary>
        /// 队列或者Topic
        /// </summary>
        public string Destination { get; set; }
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
        /// <summary>
        /// 保留发布者数
        /// </summary>
        public int InitializeCount { get; set; } = 5;
        /// <summary>
        /// 是否使用事务
        /// </summary>
        public bool Transactional { get; set; }
    }
}
