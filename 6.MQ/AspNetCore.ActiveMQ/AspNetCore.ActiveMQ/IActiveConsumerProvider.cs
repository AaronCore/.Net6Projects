using System;
using System.Threading.Tasks;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ
{
    public interface IActiveConsumerProvider : IDisposable
    {
        ActiveConsumer Consumer { get; }

        Task ListenAsync();
    }
}
