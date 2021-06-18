using VueAdmin.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace VueAdmin.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(VueAdminEntityFrameworkCoreDbMigrationsModule),
        typeof(VueAdminApplicationContractsModule)
        )]
    public class VueAdminDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
