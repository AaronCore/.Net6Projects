using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Zookeeper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.WebApi
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
                    //Address = new string[] { "192.168.209.133:2181", "192.168.209.134:2181", "192.168.209.135:2181" },
                    Address = new string[] { "192.168.209.133:2181" },
                    RootPath = "/Test",
                    ExcludeRoot = true
                });
    }
}
