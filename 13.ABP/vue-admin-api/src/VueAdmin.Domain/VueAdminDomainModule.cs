using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using VueAdmin.Domain.Shared;

namespace VueAdmin.Domain
{
    [DependsOn(
        typeof(VueAdminDomainSharedModule),
        typeof(AbpIdentityDomainModule)
        )]
    public class VueAdminDomainModule : AbpModule
    {

    }
}
