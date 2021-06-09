using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ActiveMQ.Test
{
    public abstract class BaseUnitTest
    {
        protected static string[] brokerUris = new string[] { "192.168.209.133:61616", "192.168.209.134:61616", "192.168.209.135:61616" };
        protected static string host = "192.168.209128:61616";
        protected static string userName = "test";
        protected static string password = "123456";
        protected static string queue = "queue";
        protected static string topic = "topic";
        protected static string logger = "logger";

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
