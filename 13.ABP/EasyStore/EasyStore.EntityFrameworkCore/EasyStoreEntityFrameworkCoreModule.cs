using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace EasyStore.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class EasyStoreEntityFrameworkCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<EasyStoreDbContext>(opt =>
            {
                opt.AddDefaultRepositories(true);
            });

            Configure<AbpDbContextOptions>(opt =>
            {
                opt.UseSqlServer();
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }
    }
}
