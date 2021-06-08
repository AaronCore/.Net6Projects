using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCore.RabbitMQ.Integration
{
    public class RabbitProducer : RabbitBase
    {
        public RabbitProducer(params string[] hostAndPorts) : base(hostAndPorts)
        {

        }

        #region 普通模式、Work模式
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        /// <param name="options"></param>
        public void Publish(string queue, string message, QueueOptions options = null)
        {
            Publish(queue, new string[] { message }, options);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="messages"></param>
        /// <param name="options"></param>
        public void Publish(string queue, string[] messages, QueueOptions options = null)
        {
            if (string.IsNullOrEmpty(queue))
            {
                throw new ArgumentException("queue cannot be empty", nameof(queue));
            }

            options = options ?? new QueueOptions();
            var channel = GetChannel();
            PrepareQueueChannel(channel, queue, options);

            foreach (var message in messages)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", queue, null, buffer);
            }
            channel.Close();
        }
        #endregion
        #region 订阅模式、路由模式、Topic模式
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <param name="options"></param>
        public void Publish(string exchange, string routingKey, string message, ExchangeQueueOptions options = null)
        {
            Publish(exchange, new RouteMessage() { Message = message, RoutingKey = routingKey }, options);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="messages"></param>
        /// <param name="options"></param>
        public void Publish(string exchange, string routingKey, string[] messages, ExchangeQueueOptions options = null)
        {
            Publish(exchange, messages.Select(message => new RouteMessage() { Message = message, RoutingKey = routingKey }).ToArray(), options);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routeMessage"></param>
        /// <param name="options"></param>
        public void Publish(string exchange, RouteMessage routeMessage, ExchangeQueueOptions options = null)
        {
            Publish(exchange, new RouteMessage[] { routeMessage }, options);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routeMessages"></param>
        /// <param name="options"></param>
        public void Publish(string exchange, RouteMessage[] routeMessages, ExchangeQueueOptions options = null)
        {
            if (string.IsNullOrEmpty(exchange))
            {
                throw new ArgumentException("exchange cannot be empty", nameof(exchange));
            }
            options = options ?? new ExchangeQueueOptions();
            if (options.Type == RabbitExchangeType.None)
            {
                throw new NotSupportedException($"{nameof(RabbitExchangeType)} must be specified");
            }

            var channel = GetChannel();
            PrepareExchangeChannel(channel, exchange, options);

            foreach (var routeMessage in routeMessages)
            {
                var buffer = Encoding.UTF8.GetBytes(routeMessage.Message);
                channel.BasicPublish(exchange, routeMessage.RoutingKey, null, buffer);
            }
            channel.Close();
        }
        #endregion

        public static RabbitProducer Create(RabbitBaseOptions rabbitBaseOptions)
        {
            var producer = new RabbitProducer(rabbitBaseOptions.Hosts);
            producer.Password = rabbitBaseOptions.Password;
            producer.Port = rabbitBaseOptions.Port;
            producer.UserName = rabbitBaseOptions.UserName;
            producer.VirtualHost = rabbitBaseOptions.VirtualHost;
            return producer;
        }
        public override void Dispose()
        {
            base.Dispose();
            Close();
        }
    }
    public class RouteMessage
    {
        /// <summary>
        /// 路由
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
