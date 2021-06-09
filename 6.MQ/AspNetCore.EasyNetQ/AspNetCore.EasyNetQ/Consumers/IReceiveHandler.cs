using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ.Consumers
{
    public interface IReceiveHandler
    {
        Guid Id { get; }
        string Queue { get; }

        /// <summary>
        /// 添加消息处理
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        Task ResolveAsync(IReceiveRegistration registration);
    }
}
