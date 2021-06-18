using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using VueAdmin.Domain;
using VueAdmin.Domain.Configurations;

namespace VueAdmin.Application.Caching
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(VueAdminDomainModule)
    )]
    public class VueAdminApplicationCachingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            if (AppSettings.Caching.IsOpen)
            {
                context.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = AppSettings.Caching.RedisConnectionString;
                });

                var csredis = new CSRedis.CSRedisClient(AppSettings.Caching.RedisConnectionString);
                RedisHelper.Initialization(csredis);

                context.Services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
            }
        }
    }
}
