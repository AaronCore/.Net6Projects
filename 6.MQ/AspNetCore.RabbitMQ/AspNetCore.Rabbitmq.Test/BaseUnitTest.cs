using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.RabbitMQ.Test
{
    public abstract class BaseUnitTest
    {
        protected static string[] hosts = new string[] { "192.168.209.133", "192.168.209.134", "192.168.209.135" };
        protected static int port = 5672;
        protected static string userName = "test";
        protected static string password = "123456";
        protected static string virtualHost = "/test";
        protected static string logger = "logger";
        protected static string queue1 = "queue1";
        protected static string queue2 = "queue2";
        protected static string topic = "test.topic";
        protected static string fanout = "test.fanout";
        protected static string direct = "test.direct";
        protected static Dictionary<string, object> arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };

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
