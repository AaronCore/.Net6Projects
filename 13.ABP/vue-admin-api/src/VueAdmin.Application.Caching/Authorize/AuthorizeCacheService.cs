using System;
using System.Threading.Tasks;
using VueAdmin.Common.Base;
using VueAdmin.Common.Extensions;
using VueAdmin.Domain.Shared;

namespace VueAdmin.Application.Caching.Authorize
{
    public class AuthorizeCacheService : VueAdminApplicationCachingService, IAuthorizeCacheService
    {
        private const string Authorize_Prefix = VueAdminConsts.CachePrefix.Authorize;

        private const string KEY_GetLoginAddress = Authorize_Prefix + ":GetLoginAddress";
        private const string KEY_GetAccessToken = Authorize_Prefix + ":GetAccessToken-{0}";
        private const string KEY_GenerateToken = Authorize_Prefix + ":GenerateToken-{0}";

        /// <summary>
        /// 获取登录地址(GitHub)
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public async Task<ServiceResult<string>> GetLoginAddressAsync(Func<Task<ServiceResult<string>>> factory)
        {
            return await Cache.GetOrAddAsync(KEY_GetLoginAddress, factory, VueAdminConsts.CacheStrategy.NEVER);
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="code"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public async Task<ServiceResult<string>> GetAccessTokenAsync(string code, Func<Task<ServiceResult<string>>> factory)
        {
            return await Cache.GetOrAddAsync(KEY_GetAccessToken.FormatWith(code), factory, VueAdminConsts.CacheStrategy.FIVE_MINUTES);
        }

        /// <summary>
        /// 登录成功，生成Token
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public async Task<ServiceResult<string>> GenerateTokenAsync(string access_token, Func<Task<ServiceResult<string>>> factory)
        {
            return await Cache.GetOrAddAsync(KEY_GenerateToken.FormatWith(access_token), factory, VueAdminConsts.CacheStrategy.HALF_HOURS);
        }
    }
}
