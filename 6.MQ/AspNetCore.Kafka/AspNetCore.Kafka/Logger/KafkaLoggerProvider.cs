using AspNetCore.Kafka.Integration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Kafka.Logger
{
    public class KafkaLoggerProvider : ILoggerProvider
    {
        KafkaLoggerOptions loggerOptions;
        KafkaProducer producer;

        public KafkaLoggerProvider(IOptionsMonitor<KafkaLoggerOptions> options)
        {
            loggerOptions = options.CurrentValue;

            producer = KafkaProducer.Create(loggerOptions);
            producer.DefaultKey = loggerOptions.Key;
            producer.DefaultTopic = loggerOptions.Topic;
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
            return new KafkaLogger(categoryName, loggerOptions, producer);
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
