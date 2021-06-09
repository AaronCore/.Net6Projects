using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.WebApi.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        IRabbitProducerFactory rabbitProducerFactory;
        ILogger<HomeController> logger;

        public HomeController(IRabbitProducerFactory rabbitProducerFactory, ILogger<HomeController> logger)
        {
            this.logger = logger;
            this.rabbitProducerFactory = rabbitProducerFactory;
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
        /// Simple
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Simple")]
        public string Simple(string message = "Simple")
        {
            var producer = rabbitProducerFactory.Create("SimplePattern");
            producer.Publish(message);

            return "success";
        }
        /// <summary>
        /// Worker
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Worker")]
        public string Worker(string message = "Worker")
        {
            var producer = rabbitProducerFactory.Create("WorkerPattern");
            int count = 10;
            while (count-- > 0)
            {
                producer.Publish(message);
            }

            return "success";
        }
        /// <summary>
        /// Direct
        /// </summary>
        /// <param name="route"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Direct")]
        public string Direct(string route = "direct1", string message = "Direct")
        {
            var producer = rabbitProducerFactory.Create("DirectPattern");
            producer.Publish(route, message);

            return "success";
        }
        /// <summary>
        /// Fanout
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Fanout")]
        public string Fanout(string message = "Fanout")
        {
            var producer = rabbitProducerFactory.Create("FanoutPattern");
            producer.Publish("", message);//fanout模式路由为空值

            return "success";
        }
        /// <summary>
        /// Topic
        /// </summary>
        /// <param name="route"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Topic")]
        public string Topic(string route = "topic1.a", string message = "Topic")
        {
            var producer = rabbitProducerFactory.Create("TopicPattern");
            producer.Publish(route, message);

            return "success";
        }
    }
}
