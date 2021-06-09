using AspNetCore.Kafka;
using AspNetCore.Kafka.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Consumer
{
    public class KafkaConsumerListener : IKafkaConsumerListener
    {
        public Task ConsumeAsync(RecieveResult recieveResult)
        {
            Console.WriteLine("KafkaConsumerListener:" + recieveResult.Message);
            recieveResult.Commit();
            return Task.CompletedTask;
        }
    }
}
