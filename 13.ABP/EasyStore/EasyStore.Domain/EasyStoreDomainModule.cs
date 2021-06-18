using System;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace EasyStore.Domain
{
    [DependsOn(
        typeof(AbpDddDomainModule)
        )]
    public class EasyStoreDomainModule : AbpModule
    {

    }
}
