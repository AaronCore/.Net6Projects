using AspNetCore.ActiveMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ActiveMQ.Logger;

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
            var brokerUris = new string[] { "192.168.209.133:61616", "192.168.209.134:61616", "192.168.209.135:61616" };
            string userName = "test";
            string password = "123456";

            #region ÈÕÖ¾¼ÇÂ¼

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            services.AddActiveLogger(options =>
            {
                options.IsCluster = true;
                options.ApplicationName = "WebApi";
                options.BrokerUris = brokerUris;
                options.Category = "Home";
                options.UseQueue = false;
                options.Destination = "logger";
                options.MinLevel = LogLevel.Warning;
                options.InitializeCount = 10;
                options.IsPersistent = true;
                options.Password = password;
                options.UserName = userName;
            });

            #endregion
            #region Active

            services.AddActiveProducer("active.queue", options =>
            {
                options.IsCluster = true;
                options.BrokerUris = brokerUris;
                options.Destination = "active.queue";
                options.IsPersistent = true;
                options.Transactional = false;
                options.Password = password;
                options.UserName = userName;
            });
            services.AddActiveProducer("active.topic", options =>
            {
                options.IsCluster = true;
                options.BrokerUris = brokerUris;
                options.Destination = "active.topic";
                options.IsPersistent = true;
                options.Transactional = false;
                options.Password = password;
                options.UserName = userName;
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
