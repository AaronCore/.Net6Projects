using System;
using Log4Net.Sample.Common;

namespace Log4Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start...");

            //注册log4net
            Logger.Instance().Register();
            Logger.Info("hello word...");

            Console.WriteLine("End...");
            Console.ReadKey();
        }
    }
}
