using System;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ
{
    public interface IActiveConsumerBuilder
    {
        IServiceCollection Services { get; }

        IActiveConsumerBuilder AddListener(Action<IServiceProvider, RecieveResult> onMessageRecieved);
    }
}
