using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ.Test
{
    public abstract class BaseUnitTest
    {
        protected static string connectionString = "host=192.168.209.133;virtualHost=/test;username=test;password=123456;timeout=60";
        protected static string[] hosts = new string[] { "192.168.209.133", "192.168.209.134", "192.168.209.135" };
        protected static ushort port = 5672;
        protected static string userName = "test";
        protected static string password = "123456";
        protected static string virtualHost = "/test";
        protected static Dictionary<string, object> arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };
        protected static string topic = "topic";
        protected static string queue = "queue";

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
