using AspNetCore.RabbitMQ.Integration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Consumers
{
    public interface IRabbitConsumerBuilder
    {
        IServiceCollection Services { get; }

        /// <summary>
        /// 添加一个队列监听
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        IRabbitConsumerBuilder AddListener(string queue, Action<IServiceProvider, RecieveResult> onMessageRecieved);
        /// <summary>
        /// 添加一个交换机监听
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        IRabbitConsumerBuilder AddListener(string exchange, string queue, Action<IServiceProvider, RecieveResult> onMessageRecieved);
    }
}
