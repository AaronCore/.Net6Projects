using AspNetCore.RabbitMQ.Integration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Logger
{
    public class RabbitLoggerProvider : BaseProducerPool, ILoggerProvider
    {
        RabbitLoggerOptions loggerOptions;

        public RabbitLoggerProvider(IOptionsMonitor<RabbitLoggerOptions> options) : base(options.CurrentValue)
        {
            loggerOptions = options.CurrentValue;
        }

        protected override int InitializeCount => loggerOptions.InitializeCount;

        /// <summary>
        /// 创建Logger对象
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            //可缓存实例，这里略过了
            return new RabbitLogger(categoryName, loggerOptions, this);
        }
    }
}
