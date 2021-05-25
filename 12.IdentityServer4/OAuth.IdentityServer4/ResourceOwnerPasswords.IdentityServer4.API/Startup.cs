using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ResourceOwnerPasswords.IdentityServer4.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAuthorization(); // 授权

            // 身份认证
            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(setup =>
            {
                setup.Authority = "http://localhost:5001";
                setup.RequireHttpsMetadata = false;
                setup.ApiName = "api1";
                setup.ApiSecret = "apipassword"; // 对应ApiResources中的密钥
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication(); // 身份认证
            app.UseAuthorization();  // 授权

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
