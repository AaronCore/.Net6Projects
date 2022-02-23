using System;
using EasyStore.EntityFrameworkCore;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace EasyStore.Application
{
    [DependsOn(
        typeof(EasyStoreEntityFrameworkCoreModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    public class EasyStoreApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<EasyStoreApplicationAutoMapperProfile>();
            });
        }
    }
}
