using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Zookeeper.ZookeeperLibrary;

namespace WebApp.Zookeeper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseZookeeper(new ZookeeperOptions()
                {
                    //Auth = "user:123",
                    //Scheme = AuthScheme.Digest,
                    Address = new string[] { "127.0.0.1:2181" },
                    RootPath = "/Test",
                    ExcludeRoot = true
                });
    }
}
