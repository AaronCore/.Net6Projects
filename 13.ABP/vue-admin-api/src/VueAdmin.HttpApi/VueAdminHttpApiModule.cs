using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using VueAdmin.Application;

namespace VueAdmin.HttpApi
{
    [DependsOn(
        typeof(AbpIdentityHttpApiModule),
        typeof(VueAdminApplicationModule)
        )]
    public class VueAdminHttpApiModule : AbpModule
    {

    }
}
