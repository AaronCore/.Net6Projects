using AspNetCore.Kafka.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Kafka
{
    public interface IKafkaConsumerProvider : IDisposable
    {
        KafkaConsumer Consumer { get; }

        Task ListenAsync();
    }
}
