using AspNetCore.ActiveMQ;
using AspNetCore.ActiveMQ.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Consumer
{
    public class MyActiveConsumerListener : IActiveConsumerListener
    {
        public Task ConsumeAsync(RecieveResult result)
        {
            Console.WriteLine("Message From topic:" + result.Message);
            result.Commit();
            return Task.CompletedTask;
        }
    }
}
