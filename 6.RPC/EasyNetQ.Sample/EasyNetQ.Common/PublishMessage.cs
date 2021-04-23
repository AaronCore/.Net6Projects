using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNetQ.Common
{
    public class PublishMessage
    {
        /// <summary>
        /// 发送Json
        /// </summary>
        public object SendMessage { get; set; }
        /// <summary>
        /// 消息推送的模式
        /// 现在支持：订阅模式,推送模式,主题路由模式
        /// </summary>
        public SendEnum SendEnum { get; set; }
        /// <summary>
        /// 管道名称
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; } = "";
    }
}
