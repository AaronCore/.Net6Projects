using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VueAdmin.Application.Caching.Authorize;
using VueAdmin.Application.EventBus;

namespace VueAdmin.Application.Caching.EventHandlers
{
    public class AuthorizeCachingRemoveHandler : IDistributedEventHandler<CachingRemoveEventData>, ITransientDependency
    {
        private readonly IAuthorizeCacheService _authorizeCacheService;

        public AuthorizeCachingRemoveHandler(IAuthorizeCacheService authorizeCacheService)
        {
            _authorizeCacheService = authorizeCacheService;
        }

        /// <summary>
        /// 清除缓存操作，指定Key前缀
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public async Task HandleEventAsync(CachingRemoveEventData eventData)
        {
            await _authorizeCacheService.RemoveAsync(eventData.Key);
        }
    }
}
