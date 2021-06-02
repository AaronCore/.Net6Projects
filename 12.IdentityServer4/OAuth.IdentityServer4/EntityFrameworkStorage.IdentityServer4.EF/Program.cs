using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace EntityFrameworkStorage.IdentityServer4.EF
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //执行： dotnet run /seed 初始化数据
            var seed = args.Contains("/seed");
            if (seed)
            {
                args = args.Except(new[] { "/seed" }).ToArray();
            }
            var host = CreateHostBuilder(args).Build();
            if (seed)
            {
                SeedData.EnsureSeedData(host.Services);
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
