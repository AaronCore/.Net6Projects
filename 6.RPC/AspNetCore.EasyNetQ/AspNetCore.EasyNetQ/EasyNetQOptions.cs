using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCore.EasyNetQ
{
    public class EasyNetQOptions
    {
        /// <summary>
        /// 连接字符串(优先使用连接字符串)
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 服务节点
        /// </summary>
        public string[] Hosts { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public ushort Port { get; set; } = 5672;
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 虚拟机
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        /// <summary>
        /// 心跳设置
        /// </summary>
        public TimeSpan RequestedHearbeat { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// 在EasyNetQ发送ack之前发送给RabbitMQ的消息数
        /// </summary>
        public ushort PrefetchCount { get; set; } = 50;
        /// <summary>
        /// 默认为true。这个决定了在发送消息时采用什么样的delivery_mode。设置为true，RabbitMQ将会把消息持久化到磁盘，并且在服务器重启后仍会存在。设置为false可以提高性能收益。
        /// </summary>
        public bool PersistentMessages { get; set; } = true;
        /// <summary>
        /// 模式值为10秒。不限制超时时间设置为0.当超时事时抛出System.TimeoutException.
        /// </summary>
        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        internal IBus CreateBus()
        {
            EasyNetQOptions easyNetQOptions = this;
            IBus bus = null;

            if (!string.IsNullOrEmpty(easyNetQOptions.ConnectionString))
            {
                bus = RabbitHutch.CreateBus(easyNetQOptions.ConnectionString);
            }
            else
            {
                var hosts = easyNetQOptions.Hosts.Select(f =>
                {
                    var split = f.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    HostConfiguration host = new HostConfiguration();
                    host.Host = split.FirstOrDefault();
                    host.Port = split.Length > 1 ? ushort.Parse(split.ElementAt(1)) : easyNetQOptions.Port;
                    return host;
                }).ToArray();

                ConnectionConfiguration connection = new ConnectionConfiguration();
                connection.Port = easyNetQOptions.Port;
                connection.Password = easyNetQOptions.Password;
                connection.UserName = easyNetQOptions.UserName;
                connection.VirtualHost = easyNetQOptions.VirtualHost;
                connection.Timeout = easyNetQOptions.TimeOut;
                connection.PrefetchCount = easyNetQOptions.PrefetchCount;
                connection.RequestedHeartbeat = easyNetQOptions.RequestedHearbeat;
                connection.PersistentMessages = easyNetQOptions.PersistentMessages;
                connection.Hosts = hosts;

                bus = RabbitHutch.CreateBus(connection, services => { });
            }
            return bus;
        }
    }
}
