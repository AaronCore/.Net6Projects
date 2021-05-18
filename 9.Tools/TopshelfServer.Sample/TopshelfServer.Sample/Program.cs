using System;
using Topshelf;

namespace TopshelfServer.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
                {
                    x.Service<LoggingService>();
                    x.EnableServiceRecovery(r => r.RestartService(TimeSpan.FromSeconds(10)));
                    x.SetServiceName("TopshelfTestService");
                    x.StartAutomatically();
                }
            );
        }
    }
    public class LoggingService : ServiceControl
    {
        private void Log(string logMessage)
        {
            Console.WriteLine(logMessage);
        }

        public bool Start(HostControl hostControl)
        {
            Log("Starting");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Log("Stopping");
            return true;
        }
    }
}
