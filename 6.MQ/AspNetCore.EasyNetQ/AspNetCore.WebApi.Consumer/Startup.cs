using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EasyNetQ;
using AspNetCore.WebApi.Message;
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
            var connectionString = "host=192.168.209.133;virtualHost=/;username=admin;password=123456;timeout=60";
            string[] hosts = new string[] { "192.168.209.133", "192.168.209.134", "192.168.209.135" };
            ushort port = 5672;
            string userName = "admin";
            string password = "123456";
            string virtualHost = "/";
            var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };

            #region 订阅发布

            services.AddEasyNetQConsumer(options =>
            {
                //options.ConnectionString = connectionString;
                options.Hosts = hosts;
                options.Port = port;
                options.Password = password;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.AutoDelete = true;
                options.Durable = true;
                options.PrefetchCount = 1;
                options.Priority = 2;
            })
            .AddSubscriber("PubSub1",typeof(EasyNetQSubscriber))
            .AddSubscriber<Subscriber>("PubSub2", r =>
            {
                Console.WriteLine("PubSub:" + r.Message);
            });

            #endregion
            #region 请求响应

            services.AddEasyNetQConsumer(options =>
            {
                //options.ConnectionString = connectionString;
                options.Hosts = hosts;
                options.Port = port;
                options.Password = password;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Durable = true;
                options.PrefetchCount = 2;
            })
            .AddResponder(typeof(EasyNetQResponder))
            .AddResponder<Requester, Responder>(request =>
            {
                Console.WriteLine("Rpc:" + request.Data);
                return new Responder() { Result = "Rpc:" + request.Data };
            });

            #endregion
            #region 发送接收

            services.AddEasyNetQConsumer(options =>
            {
                //options.ConnectionString = connectionString;
                options.Hosts = hosts;
                options.Port = port;
                options.Password = password;
                options.UserName = userName;
                options.VirtualHost = virtualHost;

                options.Priority = 5;
                options.PrefetchCount = 5;
                options.Exclusive = false;
                options.Arguments = arguments;
                options.Queue = "send-recieve";
            })
            .AddReceiver(typeof(EasyNetQReceiver<Reciever1>))
            .AddReceiver(typeof(EasyNetQReceiver<Reciever2>));
            //.AddReceiver<Reciever1>(r =>
            //{
            //    Console.WriteLine("Reciever1:" + r.Message);
            //})
            //.AddReceiver<Reciever2>(r =>
            //{
            //    Console.WriteLine("Reciever2:" + r.Message);
            //});

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
