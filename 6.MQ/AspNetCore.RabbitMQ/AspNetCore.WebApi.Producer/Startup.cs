using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.RabbitMQ;
using AspNetCore.RabbitMQ.Integration;
using AspNetCore.RabbitMQ.Logger;
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
            string[] hosts = new string[] { "192.168.209.133", "192.168.209.134", "192.168.209.135" };
            int port = 5672;
            string userName = "admin";
            string password = "123456";
            string virtualHost = "/";
            var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };

            #region 日志记录

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            services.AddRabbitLogger(options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.Category = "Home";
                options.MinLevel = LogLevel.Trace;
                options.ApplicationName = "AspNetCore.WebApi.Producer";

                //队列模式
                options.Queue = "queue.logger";

                //交换机模式
                //options.Exchange = "exchange.logger";
                //options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.logger", Route = "#" } };
                //options.Type = RabbitExchangeType.Topic;
            });

            #endregion

            #region 普通模式

            services.AddRabbitProducer("SimplePattern", options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.InitializeCount = 3;
                options.Queues = new string[] { "queue.simple" };
            });

            #endregion

            #region 工作模式

            services.AddRabbitProducer("WorkerPattern", options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.InitializeCount = 3;
                options.Queues = new string[] { "queue.worker" };
            });

            #endregion

            #region 发布订阅模式

            services.AddRabbitProducer("FanoutPattern", options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.InitializeCount = 3;
                options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.fanout1" }, new RouteQueue() { Queue = "queue.fanout2" } };
                options.Type = RabbitExchangeType.Fanout;
                options.Exchange = "exchange.fanout";
            });

            #endregion

            #region 路由模式

            services.AddRabbitProducer("DirectPattern", options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.InitializeCount = 5;
                options.Exchange = "exchange.direct";
                options.Type = RabbitExchangeType.Direct;
                options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.direct1", Route = "direct1" }, new RouteQueue() { Queue = "queue.direct2", Route = "direct2" } };
            });

            #endregion

            #region 主题模式

            services.AddRabbitProducer("TopicPattern", options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.InitializeCount = 5;
                options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.topic1", Route = "topic1.#" }, new RouteQueue() { Queue = "queue.topic2", Route = "topic2.#" } };
                options.Type = RabbitExchangeType.Topic;
                options.Exchange = "exchange.topic";
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
