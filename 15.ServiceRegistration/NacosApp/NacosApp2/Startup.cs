using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.V2.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nacos.AspNetCore.V2;

namespace NacosApp2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNacosV2Config(x =>
            {
                x.ServerAddresses = new List<string> { "http://localhost:8848/" };
                x.Namespace = "00446a46-3b10-4765-8534-6c32e0f84945";
                x.UserName = "nacos";
                x.Password = "nacos";
                x.EndPoint = "";

                // this sample will add the filter to encrypt the config with AES.
                x.ConfigFilterAssemblies = new List<string> { "App3" };

                // swich to use http or rpc
                x.ConfigUseRpc = true;
            });

            services.AddNacosV2Naming(x =>
            {
                x.ServerAddresses = new List<string> { "http://localhost:8848/" };
                x.Namespace = "00446a46-3b10-4765-8534-6c32e0f84945";
                x.UserName = "nacos";
                x.Password = "nacos";
                x.EndPoint = "";

                // swich to use http or rpc
                x.NamingUseRpc = true;
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
