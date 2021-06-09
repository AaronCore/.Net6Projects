using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.WebApi.Producer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "host=192.168.209.133;virtualHost=/;username=admin;password=123456;timeout=60";
            string[] hosts = new string[] { "192.168.209.133", "192.168.209.134", "192.168.209.135" };
            ushort port = 5672;
            string userName = "admin";
            string password = "123456";
            string virtualHost = "/";
            var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };

            #region 订阅发布

            services.AddEasyNetQProducer("Publish", options =>
            {
                //options.ConnectionString = connectionString;
                options.Hosts = hosts;
                options.Port = port;
                options.Password = password;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.PersistentMessages = true;
                options.Priority = 1;
            });

            #endregion
            #region 请求响应

            services.AddEasyNetQProducer("Request", options =>
            {
                //options.ConnectionString = connectionString;
                options.Hosts = hosts;
                options.Port = port;
                options.Password = password;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.PersistentMessages = true;
                options.Priority = 3;
            });

            #endregion
            #region 发送接收

            services.AddEasyNetQProducer("Send", options =>
            {
                //options.ConnectionString = connectionString;
                options.Hosts = hosts;
                options.Port = port;
                options.Password = password;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Priority = 4;
                options.Queue = "send-recieve";
            });

            #endregion

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "WebApi"
                });
                var assembly = typeof(Startup).Assembly;
                var path = Path.Combine(Path.GetDirectoryName(assembly.Location), $"{assembly.GetName().Name}.xml");
                options.IncludeXmlComments(path, true);
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi");
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
