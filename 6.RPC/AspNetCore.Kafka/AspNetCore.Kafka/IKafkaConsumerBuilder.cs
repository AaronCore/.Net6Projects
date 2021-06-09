using AspNetCore.Kafka.Integration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Kafka
{
    public interface IKafkaConsumerBuilder
    {
        IServiceCollection Services { get; }

        IKafkaConsumerBuilder AddListener(Action<IServiceProvider, RecieveResult> onMessageRecieved);
    }
}
