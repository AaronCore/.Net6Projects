using System;
using System.Threading.Tasks;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ
{
    public interface IActiveClientProducer : IDisposable
    {
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <returns></returns>
        Task SendAsync(params ActiveMessage[] activeMessages);
        /// <summary>
        /// 发送消息到Topic
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <returns></returns>
        Task PublishAsync(params ActiveMessage[] activeMessages);
    }
}
