using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.ActiveMQ.Logger
{
    public static class ActiveLoggerExtensions
    {
        /// <summary>
        /// 添加ActiveMq日志记录
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddActiveLogger(this IServiceCollection services, Action<ActiveLoggerOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<ILoggerProvider, ActiveLoggerProvider>();

            return services;
        }
        /// <summary>
        /// 添加ActiveMq日志记录
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddActiveLogger(this ILoggingBuilder builder, Action<ActiveLoggerOptions> configure)
        {
            builder.Services.AddActiveLogger(configure);
            return builder;
        }
    }
}
