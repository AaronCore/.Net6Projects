using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ
{
    public interface IEasyNetQResponder<TRequest, TResponse>
            where TRequest : class
            where TResponse : class
    {
        /// <summary>
        /// 响应事件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<TResponse> RespondAsync(TRequest request);
    }
}
