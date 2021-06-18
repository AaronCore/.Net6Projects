using System;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.SqlServer;
using Volo.Abp;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Modularity;
using VueAdmin.Application.Contracts;
using VueAdmin.Domain.Configurations;
using VueAdmin.Domain.Shared;

namespace VueAdmin.Jobs
{
    [DependsOn(
        typeof(AbpBackgroundJobsHangfireModule),
        typeof(VueAdminApplicationContractsModule)
    )]
    public class VueAdminJobsModule: AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHangfire(config =>
            {
                var tablePrefix = VueAdminConsts.DbTablePrefix + "hangfire";

                config.UseSqlServerStorage(AppSettings.ConnectionStrings, new SqlServerStorageOptions
                {
                    SchemaName = tablePrefix
                });
            });
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();


            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = false,
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        Users = new []
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = AppSettings.Hangfire.Login,
                                PasswordClear =  AppSettings.Hangfire.Password
                            }
                        }
                    })
                },
                DashboardTitle = "任务调度中心"
            });

            var service = context.ServiceProvider;

            service.UseHangfireTest();

            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }
    }
}
