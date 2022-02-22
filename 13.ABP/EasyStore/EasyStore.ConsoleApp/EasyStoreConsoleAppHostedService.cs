using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace EasyStore.ConsoleApp
{
    public class EasyStoreConsoleAppHostedService : IHostedService
    {
        private IAbpApplicationWithInternalServiceProvider _abpApplication = null!;

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public EasyStoreConsoleAppHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _abpApplication = await AbpApplicationFactory.CreateAsync<EasyStoreConsoleAppAppModule>(options =>
            {
                options.Services.ReplaceConfiguration(_configuration);
                options.Services.AddSingleton(_hostEnvironment);

                options.UseAutofac();
                options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
            });

            await _abpApplication.InitializeAsync();

            var helloWorldService = _abpApplication.ServiceProvider.GetRequiredService<HelloWorldService>();

            await helloWorldService.SayHelloAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.ShutdownAsync();
        }
    }
}
