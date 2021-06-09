using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ
{
    public interface IEasyNetQConsumerProvider : IDisposable
    {
        /// <summary>
        /// 开始监听消费消息
        /// </summary>
        /// <returns></returns>
        Task ListenAsync();
    }
}
