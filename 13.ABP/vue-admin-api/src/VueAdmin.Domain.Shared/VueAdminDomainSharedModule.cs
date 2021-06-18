using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace VueAdmin.Domain.Shared
{
    [DependsOn(
        typeof(AbpIdentityDomainSharedModule)
        )]
    public class VueAdminDomainSharedModule : AbpModule
    {

    }
}
