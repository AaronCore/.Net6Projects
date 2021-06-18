using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using VueAdmin.EntityFrameworkCore.EntityFrameworkCore;

namespace VueAdmin.EntityFrameworkCore.DbMigrations.EntityFrameworkCore
{
    [DependsOn(
        typeof(VueAdminEntityFrameworkCoreModule)
        )]
    public class VueAdminEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<VueAdminMigrationsDbContext>();
        }
    }
}
