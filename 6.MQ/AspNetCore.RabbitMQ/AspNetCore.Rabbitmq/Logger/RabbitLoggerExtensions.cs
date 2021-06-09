using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Logger
{
    public static class RabbitLoggerExtensions
    {
        /// <summary>
        /// 添加rabbitmq日志记录
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitLogger(this IServiceCollection services, Action<RabbitLoggerOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<ILoggerProvider, RabbitLoggerProvider>();

            return services;
        }
        /// <summary>
        /// 添加rabbitmq日志记录
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddRabbitLogger(this ILoggingBuilder builder, Action<RabbitLoggerOptions> configure)
        {
            builder.Services.AddRabbitLogger(configure);
            return builder;
        }
    }
}
