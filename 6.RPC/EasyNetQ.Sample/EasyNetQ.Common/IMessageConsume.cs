using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNetQ.Common
{
    public interface IMessageConsume
    {
        void Consume(string message);
    }
}
