using System;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;
using Volo.Abp.RabbitMQ;
using VueAdmin.Domain;
using VueAdmin.Domain.Configurations;

namespace VueAdmin.Application.EventBus
{
    [DependsOn(
        typeof(AbpEventBusRabbitMqModule),
        typeof(VueAdminDomainModule)
    )]
    public class VueAdminApplicationEventBusModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDistributedEntityEventOptions>(options =>
            {
                options.AutoEventSelectors.AddAll();
            });

            Configure<AbpRabbitMqOptions>(options =>
            {
                options.Connections.Default.UserName = AppSettings.RabbitMQ.Connections.Default.Username;
                options.Connections.Default.Password = AppSettings.RabbitMQ.Connections.Default.Password;
                options.Connections.Default.HostName = AppSettings.RabbitMQ.Connections.Default.HostName;
                options.Connections.Default.Port = AppSettings.RabbitMQ.Connections.Default.Port;
            });

            Configure<AbpRabbitMqEventBusOptions>(options =>
            {
                options.ClientName = AppSettings.RabbitMQ.EventBus.ClientName;
                options.ExchangeName = AppSettings.RabbitMQ.EventBus.ExchangeName;
            });
        }
    }
}
