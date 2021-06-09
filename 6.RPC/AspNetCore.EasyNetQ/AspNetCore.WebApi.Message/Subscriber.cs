using AspNetCore.EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Message
{
    public class Subscriber
    {
        public string Message { get; set; }
    }
    public class EasyNetQSubscriber : IEasyNetQSubscriber<Subscriber>
    {
        public Task SubscribeAsync(Subscriber message)
        {
            Console.WriteLine("EasyNetQSubscriber:" + message.Message);
            return Task.CompletedTask;
        }
    }
}
