using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.RabbitMQ;
using AspNetCore.RabbitMQ.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            string[] hosts = new string[] { "192.168.209.133", "192.168.209.134", "192.168.209.135" };
            int port = 5672;
            string userName = "admin";
            string password = "123456";
            string virtualHost = "/";
            var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };

            #region 日志记录

            services.AddRabbitConsumer(options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.AutoAck = true;

                //options.FetchCount = 10;
                //options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.logger", Route = "#" } };//交换机模式
                //options.Type = RabbitExchangeType.Topic;//交换机模式
            })

            //.AddListener("queue.logger", result =>
            //{
            //    Console.WriteLine("Message From queue.logger:" + result.Body);
            //});

            .AddListener<RabbitConsumerListener>("queue.logger");

            //.AddListener("exchange.logger", "queue.logger", result =>
            //{
            //    Console.WriteLine("Message From queue.logger:" + result.Body);
            //});//交换机模式

            #endregion

            #region 普通模式

            services.AddRabbitConsumer(options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                //options.FetchCount = 1;
                options.AutoAck = false;
            }).AddListener("queue.simple", result =>
            {
                Console.WriteLine("Message From queue.simple:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            });
            #endregion

            #region 工作模式

            services.AddRabbitConsumer(options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.FetchCount = 2;
                options.AutoAck = false;
            }).AddListener("queue.worker", result =>
            {
                Console.WriteLine("Message From queue.worker1:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            }).AddListener("queue.worker", result =>
            {
                Console.WriteLine("Message From queue.worker2:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            });
            #endregion

            #region 发布订阅模式

            services.AddRabbitConsumer(options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.FetchCount = 2;
                options.AutoAck = false;
                options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.fanout1" }, new RouteQueue() { Queue = "queue.fanout2" } };
                options.Type = RabbitExchangeType.Fanout;
            }).AddListener("exchange.fanout", "queue.fanout1", result =>
            {
                Console.WriteLine("Message From queue.fanout1:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            }).AddListener("exchange.fanout", "queue.fanout2", result =>
            {
                Console.WriteLine("Message From queue.fanout2:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            });

            #endregion

            #region 路由模式

            services.AddRabbitConsumer(options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.AutoAck = false;
                options.FetchCount = 2;
                options.Type = RabbitExchangeType.Direct;
                options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.direct1", Route = "direct1" }, new RouteQueue() { Queue = "queue.direct2", Route = "direct2" } };
            }).AddListener("exchange.direct", "queue.direct1", result =>
            {
                Console.WriteLine("Message From queue.direct1:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            }).AddListener("exchange.direct", "queue.direct2", result =>
            {
                Console.WriteLine("Message From queue.direct2:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            });

            #endregion

            #region 主题模式

            services.AddRabbitConsumer(options =>
            {
                options.Hosts = hosts;
                options.Password = password;
                options.Port = port;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Arguments = arguments;
                options.Durable = true;
                options.AutoDelete = true;

                options.FetchCount = 2;
                options.AutoAck = false;
                options.RouteQueues = new RouteQueue[] { new RouteQueue() { Queue = "queue.topic1", Route = "topic1.#" }, new RouteQueue() { Queue = "queue.topic2", Route = "topic2.#" } };
                options.Type = RabbitExchangeType.Topic;
            }).AddListener("exchange.topic", "queue.topic1", result =>
            {
                Console.WriteLine("Message From queue.topic1:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            }).AddListener("exchange.topic", "queue.topic2", result =>
            {
                Console.WriteLine("Message From queue.topic2:" + result.Body);
                result.Commit();
                //result.RollBack();//回滚，参数表示是否重新消费
            });

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
