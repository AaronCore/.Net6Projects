using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Kafka.Logger
{
    public static class KafkaLoggerExtensions
    {
        /// <summary>
        /// 添加kafka日志记录
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaLogger(this IServiceCollection services, Action<KafkaLoggerOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<ILoggerProvider, KafkaLoggerProvider>();

            return services;
        }
        /// <summary>
        /// 添加kafka日志记录
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddKafkaLogger(this ILoggingBuilder builder, Action<KafkaLoggerOptions> configure)
        {
            builder.Services.AddKafkaLogger(configure);
            return builder;
        }
    }
}
