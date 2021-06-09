using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Kafka.Test
{
    public abstract class BaseUnitTest
    {
        protected static string[] hosts = new string[] { "192.168.209.133:9092", "192.168.209.134:9092", "192.168.209.135:9092" };
        protected static string topic = "topic.kafka";
        protected static string logger = "topic.logger";
        protected static string group = "group";

        protected void BlockUntil(Func<bool> func, int milliseconds)
        {
            bool signaled = false;
            Task.WaitAny(Task.Delay(milliseconds), Task.Run(() =>
            {
                while (true && !signaled)
                {
                    if (func.Invoke())
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }));
            signaled = true;
        }
    }
}
