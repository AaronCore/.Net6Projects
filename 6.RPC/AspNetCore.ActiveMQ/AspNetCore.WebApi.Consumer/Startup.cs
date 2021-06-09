using AspNetCore.Activemq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ActiveMQ;

namespace AspNetCore.WebApi.Consumer
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

            services.AddActiveConsumer(options =>
            {
                options.IsCluster = true;
                options.BrokerUris = brokerUris;
                options.ClientId = "logger";
                options.Durable = true;
                options.FromQueue = false;
                options.Destination = "logger";
                options.AutoAcknowledge = true;
                options.SubscriberName = "logger";
                options.Password = password;
                options.UserName = userName;
            }).AddListener(result =>
            {
                Console.WriteLine("Message From Topic logger:" + result.Message);
            });

            #endregion
            #region Active

            services.AddActiveConsumer(options =>
            {
                options.IsCluster = true;
                options.BrokerUris = brokerUris;
                options.Durable = false;
                options.Destination = "active.queue";
                options.AutoAcknowledge = false;
                options.FromQueue = true;
                options.Password = password;
                options.UserName = userName;
            }).AddListener(result =>
            {
                Console.WriteLine("Message From queue:" + result.Message);
                result.Commit();
            });

            services.AddActiveConsumer(options =>
            {
                options.IsCluster = true;
                options.BrokerUris = brokerUris;
                options.Durable = true;
                options.Destination = "active.topic";
                options.AutoAcknowledge = false;
                options.FromQueue = false;
                options.Password = password;
                options.UserName = userName;
                options.ClientId = "active.topic";
                options.PrefetchCount = 10;
            }).AddListener<MyActiveConsumerListener>();

            #endregion

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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
