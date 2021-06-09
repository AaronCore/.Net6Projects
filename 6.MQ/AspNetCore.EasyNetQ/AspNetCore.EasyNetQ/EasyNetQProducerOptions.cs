using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.EasyNetQ
{
    public class EasyNetQProducerOptions : EasyNetQOptions
    {
        /// <summary>
        /// 消息队列
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; } = "#";
        /// <summary>
        /// 优先等级
        /// </summary>
        public byte? Priority { get; set; }
    }
}
