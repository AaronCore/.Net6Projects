using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.WebApi.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        IKafkaProducerFactory kafkaProducerFactory;
        ILoggerFactory loggerFactory;

        public HomeController(IKafkaProducerFactory kafkaProducerFactory, ILoggerFactory loggerFactory)
        {
            this.kafkaProducerFactory = kafkaProducerFactory;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>success</returns>
        [HttpGet("Kafka")]
        public string Kafka(string message)
        {
            message = message ?? "";
            var producer = kafkaProducerFactory.Create();
            producer.Publish(message);

            return "success";
        }
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>success</returns>
        [HttpGet("Logger")]
        public string Logger(string message)
        {
            var logger1 = loggerFactory.CreateLogger("logger");
            logger1.LogTrace($"logger1(LogTrace):{message}");
            logger1.LogDebug($"logger1(LogDebug):{message}");
            logger1.LogInformation($"logger1(LogInformation):{message}");
            logger1.LogWarning($"logger1(LogWarning):{message}");
            logger1.LogError($"logger1(LogError):{message}");
            logger1.LogCritical($"logger1(LogCritical):{message}");

            var logger2 = loggerFactory.CreateLogger("123456");
            logger2.LogTrace($"logger2(LogTrace):{message}");
            logger2.LogDebug($"logger2(LogDebug):{message}");
            logger2.LogInformation($"logger2(LogInformation):{message}");
            logger2.LogWarning($"logger2(LogWarning):{message}");
            logger2.LogError($"logger2(LogError):{message}");
            logger2.LogCritical($"logger2(LogCritical):{message}");

            return "success";
        }
    }
}
