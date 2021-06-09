using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ
{
    public interface IEasyNetQReceiver<T> where T : class
    {
        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="message"></param>
        Task ReceiveAsync(T message);
    }
}
