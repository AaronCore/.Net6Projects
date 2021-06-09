using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AspNetCore.Kafka;
using AspNetCore.Kafka.Integration;
using AspNetCore.Kafka.Logger;
using System.IO;

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
            var hosts = new string[] { "192.168.209.133:9092", "192.168.209.134:9092", "192.168.209.135:9092" };

            #region ÈÕÖ¾¼ÇÂ¼

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            services.AddKafkaLogger(options =>
            {
                options.BootstrapServers = hosts;
                options.Category = "Home";
                options.InitializeCount = 10;
                options.Key = "log";
                options.MinLevel = LogLevel.Trace;
                options.Topic = "topic.logger";
                options.ApplicationName = "AspNetCore.WebApi.Producer";
            });

            #endregion

            #region Kafka

            services.AddKafkaProducer(options =>
            {
                options.BootstrapServers = hosts;
                options.InitializeCount = 3;
                options.Key = "kafka";
                options.Topic = "topic.kafka";
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
