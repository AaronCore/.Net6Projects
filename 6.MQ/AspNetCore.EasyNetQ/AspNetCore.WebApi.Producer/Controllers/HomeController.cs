using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EasyNetQ;
using AspNetCore.WebApi.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.WebApi.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        IBusClientFactory busFactory;

        public HomeController(IBusClientFactory busFactory)
        {
            this.busFactory = busFactory;
        }

        /// <summary>
        /// 发布订阅模式
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Publish")]
        public string Publish(string message)
        {
            message = message ?? "";
            var bus = busFactory.Create("Publish");
            bus.Publish(new Subscriber() { Message = message });

            return "success";
        }
        /// <summary>
        /// 请求响应模式
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Request")]
        public new string Request(string message)
        {
            message = message ?? "";
            var bus = busFactory.Create("Request");
            var response = bus.Request<Requester, Responder>(new Requester
            {
                Data = message
            });

            return response.Result;
        }
        /// <summary>
        /// 发送接收模式
        /// </summary>
        /// <param name="reciever"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet("Send")]
        public string Send(int reciever, string message)
        {
            message = message ?? "";
            var bus = busFactory.Create("Send");
            if (reciever % 2 == 1)
            {
                bus.Send(new Reciever1()
                {
                    Message = message
                });
            }
            else
            {
                bus.Send(new Reciever2()
                {
                    Message = message
                });
            }

            return "success";
        }
    }
}
