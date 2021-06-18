using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using VueAdmin.Application;
using VueAdmin.EntityFrameworkCore;
using VueAdmin.EntityFrameworkCore.EntityFrameworkCore;

namespace VueAdmin.WebApp
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpIdentityDomainSharedModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpAutofacModule),
        typeof(VueAdminEntityFrameworkCoreModule),
        typeof(VueAdminApplicationModule)
    )]
    public class VueAdminWebAppModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddControllersWithViews();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
