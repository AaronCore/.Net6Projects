﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace EasyStore.ConsoleApp
{
    public class HelloWorldService : ITransientDependency
    {
        public ILogger<HelloWorldService> Logger { get; set; }

        public HelloWorldService()
        {
            Logger = NullLogger<HelloWorldService>.Instance;
        }

        public Task SayHelloAsync()
        {
            Logger.LogInformation("Hello World!");
            return Task.CompletedTask;
        }
    }
}