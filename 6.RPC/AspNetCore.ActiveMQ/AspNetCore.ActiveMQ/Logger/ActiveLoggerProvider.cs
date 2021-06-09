using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ.Logger
{
    public class ActiveLoggerProvider : ILoggerProvider
    {
        ActiveLoggerOptions loggerOptions;
        ActiveProducer producer;

        public ActiveLoggerProvider(IOptionsMonitor<ActiveLoggerOptions> options)
        {
            loggerOptions = options.CurrentValue;

            producer = ActiveProducer.Create(loggerOptions);
            producer.InitializeCount = loggerOptions.InitializeCount;
        }

        /// <summary>
        /// 创建Logger对象
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            //可缓存实例，这里略过了
            return new ActiveLogger(categoryName, loggerOptions, producer);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            producer.Dispose();
        }
    }
}
