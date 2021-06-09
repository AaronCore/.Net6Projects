using AspNetCore.EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Message
{
    public class Reciever1
    {
        public string Message { get; set; }
    }
    public class Reciever2
    {
        public string Message { get; set; }
    }

    public class EasyNetQReceiver<T> : IEasyNetQReceiver<T> where T : class
    {
        public Task ReceiveAsync(T message)
        {
            Console.WriteLine($"EasyNetQReceiver<{typeof(T).Name}>:" + typeof(T).GetProperty("Message").GetValue(message));
            return Task.CompletedTask;
        }
    }
}
