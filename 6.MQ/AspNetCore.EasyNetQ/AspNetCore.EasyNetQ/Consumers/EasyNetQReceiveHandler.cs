using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ.Consumers
{
    public class EasyNetQReceiveHandler<T> : IReceiveHandler where T : class
    {
        Func<T, Task> onMessage;
        string queue;
        Guid id;

        public EasyNetQReceiveHandler(Guid id, string queue, Func<T, Task> onMessage)
        {
            this.id = id;
            this.queue = queue;
            this.onMessage = onMessage;
        }

        public string Queue => queue;
        public Guid Id => id;

        public Task ResolveAsync(IReceiveRegistration registration)
        {
            registration.Add<T>(onMessage);
            return Task.CompletedTask;
        }
    }
}
