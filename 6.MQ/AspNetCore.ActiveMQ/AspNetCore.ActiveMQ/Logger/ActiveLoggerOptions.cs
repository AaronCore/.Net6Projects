using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ.Logger
{
    public class ActiveLoggerOptions : ActiveBaseOptions
    {
        /// <summary>
        /// 是否使用队列，false则使用topic
        /// </summary>
        public bool UseQueue { get; set; } = true;
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
        /// 序列化方法
        /// </summary>
        public Serialize Serialize { get; set; } = value => JsonSerializer.Serialize(value);
    }
    public delegate string Serialize(object value);
}
