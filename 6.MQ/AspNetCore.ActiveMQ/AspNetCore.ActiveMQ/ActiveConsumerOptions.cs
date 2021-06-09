using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ
{
    public class ActiveConsumerOptions : ActiveBaseOptions
    {
        /// <summary>
        /// 是否使用队列，false则使用topic
        /// </summary>
        public bool FromQueue { get; set; } = true;
        /// <summary>
        /// 队列或者Topic
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// 选择器
        /// </summary>
        public string Selector { get; set; }
        /// <summary>
        /// 每次发送消息条数
        /// </summary>
        public int? PrefetchCount { get; set; }
        /// <summary>
        /// 是否自动签收
        /// </summary>
        public bool AutoAcknowledge { get; set; } = true;
        /// <summary>
        /// 重连时间间隔（秒）
        /// </summary>
        public int Interval { get; set; } = 3;
        /// <summary>
        /// 未应答时是否重发消息
        /// </summary>
        public bool RecoverWhenNotAcknowledge { get; set; } = false;
        /// <summary>
        /// 持久化订阅的客户端 Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 持久化订阅名称
        /// </summary>
        public string SubscriberName { get; set; }
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; }
    }
}
