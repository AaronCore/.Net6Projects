using AspNetCore.RabbitMQ;
using AspNetCore.RabbitMQ.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Consumer
{
    public class RabbitConsumerListener : IRabbitConsumerListener
    {
        public Task ConsumeAsync(RecieveResult recieveResult)
        {
            Console.WriteLine("RabbitConsumerListener:" + recieveResult.Body);
            recieveResult.Commit();
            //result.RollBack();//回滚，参数表示是否重新消费
            return Task.CompletedTask;
        }
    }
}
