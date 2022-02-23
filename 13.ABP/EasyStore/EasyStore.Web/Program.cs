using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EasyStore.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseAutofac();

            await builder.AddApplicationAsync<EasyStoreWebModule>();

            var app = builder.Build();

            await app.InitializeApplicationAsync();

            await app.RunAsync();
        }
    }
}