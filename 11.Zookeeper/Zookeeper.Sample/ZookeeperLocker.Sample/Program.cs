using System;
using System.Threading;

namespace ZookeeperLocker.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Zookeeper连接字符串，采用host:port格式，多个地址之间使用逗号（,）隔开
            string[] address = new string[] { "127.0.0.1:2181" };
            //会话超时时间,单位毫秒
            int sessionTimeOut = 10000;
            //锁节点根路径
            string lockerPath = "/Locker";

            for (var i = 0; i < 10; i++)
            {
                string client = "client" + i;
                //多线程模拟并发
                new Thread(() =>
                {
                    using (ZookeeperLocker zookeeperLocker = new ZookeeperLocker(lockerPath, sessionTimeOut, address))
                    {
                        string path = zookeeperLocker.CreateLock();
                        if (zookeeperLocker.Lock(path))
                        {
                            //模拟处理过程
                            Console.WriteLine($"【{client}】获得锁:{DateTime.Now}");
                            Thread.Sleep(3000);
                            Console.WriteLine($"【{client}】处理完成:{DateTime.Now}");
                        }
                        else
                        {
                            Console.WriteLine($"【{client}】获得锁失败:{DateTime.Now}");
                        }
                    }
                }).Start();
            }

            Console.ReadKey();
        }
    }
}
