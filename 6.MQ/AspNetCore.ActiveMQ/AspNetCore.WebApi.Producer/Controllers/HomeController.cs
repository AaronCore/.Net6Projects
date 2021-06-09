using AspNetCore.ActiveMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        ILogger<HomeController> logger;
        IActiveProducerFactory activeProducerFactory;
        public HomeController(ILogger<HomeController> logger, IActiveProducerFactory activeProducerFactory)
        {
            this.logger = logger;
            this.activeProducerFactory = activeProducerFactory;
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        public string Get(string message)
        {
            logger.LogTrace($"Trace:{message}");
            logger.LogDebug($"Debug:{message}");
            logger.LogInformation($"Information:{message}");
            logger.LogWarning($"Warning:{message}");
            logger.LogError($"Error:{message}");
            logger.LogCritical($"Critical:{message}");

            return "success";
        }
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>success</returns>
        [HttpGet("Queue")]
        public async Task<object> Queue(string message)
        {
            message = message ?? "";
            var producer = activeProducerFactory.Create("active.queue");
            await producer.SendAsync(message);

            return "success";
        }
        /// <summary>
        /// 发送消息到Topic
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>success</returns>
        [HttpGet("Topic")]
        public async Task<object> Topic(string message)
        {
            message = message ?? "";
            var producer = activeProducerFactory.Create("active.topic");
            await producer.PublishAsync(message);

            return "success";
        }
    }
}
