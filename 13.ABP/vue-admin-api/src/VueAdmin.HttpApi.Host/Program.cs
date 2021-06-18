using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VueAdmin.Common.Extensions;

namespace VueAdmin.HttpApi.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseLog4Net()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseIISIntegration()
                        .ConfigureKestrel(options =>
                        {
                            options.AddServerHeader = false;
                        })
                        //.UseUrls($"http://*:{AppSettings.ListenPort}")
                        .UseStartup<Startup>();
                })
                .UseAutofac();
    }
}
