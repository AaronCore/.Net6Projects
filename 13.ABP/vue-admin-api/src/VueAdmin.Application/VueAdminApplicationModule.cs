using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using VueAdmin.Application.Caching;

namespace VueAdmin.Application
{
    [DependsOn(
        typeof(AbpIdentityApplicationModule),
        typeof(VueAdminApplicationCachingModule),
        typeof(AbpAutoMapperModule)
        )]
    public class VueAdminApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<VueAdminApplicationModule>(validate: true);
                options.AddProfile<VueAdminApplicationAutoMapperProfile>(validate: true);
            });
        }
    }
}
