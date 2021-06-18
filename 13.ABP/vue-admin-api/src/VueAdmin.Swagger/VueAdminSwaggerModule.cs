using Microsoft.AspNetCore.Builder;
using Volo.Abp;
using Volo.Abp.Modularity;
using VueAdmin.Domain;

namespace VueAdmin.Swagger
{
    [DependsOn(
        typeof(VueAdminDomainModule))
    ]
    public class VueAdminSwaggerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSwagger();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.GetApplicationBuilder().UseSwagger().UseSwaggerUI();
        }
    }
}
