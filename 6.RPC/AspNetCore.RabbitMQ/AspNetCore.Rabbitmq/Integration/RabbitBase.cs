using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCore.RabbitMQ.Integration
{
    public abstract class RabbitBase : IDisposable
    {
        string[] hostAndPorts;
        IConnection connection;
        bool isDisposed = false;

        protected RabbitBase(params string[] hostAndPorts)
        {
            if (hostAndPorts == null || hostAndPorts.Length == 0)
            {
                throw new ArgumentException("invalid hostAndPorts！", nameof(hostAndPorts));
            }
            this.hostAndPorts = hostAndPorts.Select(f => f).ToArray();
        }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 5672;
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; } = ConnectionFactory.DefaultUser;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = ConnectionFactory.DefaultPass;
        /// <summary>
        /// 虚拟机
        /// </summary>
        public string VirtualHost { get; set; } = ConnectionFactory.DefaultVHost;
        /// <summary>
        /// 连接是否打开
        /// </summary>
        public bool IsOpen { get { return connection != null && connection.IsOpen; } }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            isDisposed = true;

            Close();
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (connection.IsOpen)
            {
                connection?.Close();
            }
            connection?.Dispose();
        }

        #region protected
        /// <summary>
        /// 获取rabbitmq的连接
        /// </summary>
        /// <returns></returns>
        protected IModel GetChannel()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
            if (connection == null)
            {
                lock (this)
                {
                    if (connection == null)
                    {
                        var array = hostAndPorts.Select(f => f.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
                            .Select(f => (f.FirstOrDefault(), f.Length > 1 ? int.Parse(f.ElementAt(1)) : Port))
                            .ToArray();

                        var amqpList = new List<AmqpTcpEndpoint>();
                        amqpList.AddRange(array.Select(f => new AmqpTcpEndpoint(f.Item1, f.Item2)));

                        var factory = new ConnectionFactory();
                        factory.Port = Port;
                        factory.UserName = UserName;
                        factory.VirtualHost = VirtualHost;
                        factory.Password = Password;
                        connection = factory.CreateConnection(amqpList);
                    }
                }
            }
            if (!IsOpen)
            {
                throw new ConnectFailureException("connection lost", null);
            }
            return connection.CreateModel();
        }

        /// <summary>
        /// 准备连接
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        protected void PrepareExchangeChannel(IModel channel, string exchange, ExchangeQueueOptions options)
        {
            if (options.Type == RabbitExchangeType.None)
            {
                throw new NotSupportedException($"{nameof(RabbitExchangeType)} must be specified");
            }
            channel.ExchangeDeclare(exchange, options.Type.ToString().ToLower(),
                options.Durable, options.AutoDelete, options.Arguments ?? new Dictionary<string, object>());
            if (options.RouteQueues != null)
            {
                foreach (var t in options.RouteQueues)
                {
                    if (!string.IsNullOrEmpty(t.Queue))
                    {
                        //这里的exclusive参数表示是否与当前的channel绑定
                        channel.QueueDeclare(t.Queue, t.Options?.Durable ?? options.Durable, false, t.Options?.AutoDelete ?? options.AutoDelete,
                            t.Options?.Arguments ?? options.Arguments ?? new Dictionary<string, object>());
                        channel.QueueBind(t.Queue, exchange, t.Route ?? "", options.Arguments ?? new Dictionary<string, object>());
                    }
                }
            }
        }
        /// <summary>
        /// 准备连接
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        protected void PrepareQueueChannel(IModel channel, string queue, QueueOptions options)
        {
            //这里的exclusive参数表示是否与当前的channel绑定
            channel.QueueDeclare(queue, options.Durable, false, options.AutoDelete, options.Arguments ?? new Dictionary<string, object>());
        }
        #endregion

        public override string ToString()
        {
            var array = hostAndPorts.Select(f => f.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
                            .Select(f => (f.FirstOrDefault(), f.Length > 1 ? int.Parse(f.ElementAt(1)) : Port))
                            .ToArray();
            string hosts = string.Join(",", array.Select(f => $"{f.Item1}:{f.Item2}"));
            return $"host={hosts};virtualHost={VirtualHost};username={UserName};";
        }
    }
}
