using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ
{
    public interface IEasyNetQSubscriber<T> where T : class
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="message"></param>
        Task SubscribeAsync(T message);
    }
}
