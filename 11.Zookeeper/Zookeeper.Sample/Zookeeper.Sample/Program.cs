using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using org.apache.zookeeper;
using org.apache.zookeeper.data;

namespace Zookeeper.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Zookeeper连接字符串，采用host:port格式，多个地址之间使用逗号（,）隔开
            string[] address = new string[] { "127.0.0.1:2181" };
            //会话超时时间,单位毫秒
            int sessionTimeOut = 10000;

            ZookeeperHelper zookeeperHelper = new ZookeeperHelper(address, "/");
            zookeeperHelper.SessionTimeout = sessionTimeOut;
            zookeeperHelper.Connect();//发起连接
            while (!zookeeperHelper.Connected)
            {
                Thread.Sleep(1000); //停一秒，等待连接完成
            }

            //创建znode节点
            {
                zookeeperHelper.SetData("/mynode", "hello world", true, false);
                Console.WriteLine("完成创建节点");
            }

            //节点是否存在
            {
                var exists = zookeeperHelper.Exists("/mynode");
                Console.WriteLine("节点是否存在：" + exists);
            }

            //添加监听器
            {
                zookeeperHelper.WatchAsync("/mynode", (e) =>
                {
                    Console.WriteLine($"recieve: Path-{e.Path}     State-{e.State}    Type-{e.Type}");
                }).Wait();
            }

            //获取节点数据
            {
                var value = zookeeperHelper.GetData("/mynode");
                Console.WriteLine("完成读取节点：" + value);
            }

            //设置节点数据
            {
                zookeeperHelper.SetData("/mynode", "hello world again");
                Console.WriteLine("设置节点数据");
            }

            //重新获取节点数据
            {
                var value = zookeeperHelper.GetData("/mynode");
                Console.WriteLine("重新获取节点数据：" + value);
            }

            //移除节点
            {
                zookeeperHelper.Delete("/mynode");
                Console.WriteLine("移除节点");
            }

            Console.WriteLine("完成");
            Console.ReadKey();
        }

        static void Sqmple()
        {
            //Zookeeper连接字符串，采用host:port格式，多个地址之间使用逗号（,）隔开
            string connectionString = "127.0.0.1:2181";
            //会话超时时间,单位毫秒
            int sessionTimeOut = 10000;
            //异步监听
            var watcher = new MyWatcher("ConnectWatcher");
            //连接
            ZooKeeper zooKeeper = new ZooKeeper(connectionString, sessionTimeOut, watcher);
            Thread.Sleep(1000);//停一秒，等待连接完成
            while (zooKeeper.getState() == ZooKeeper.States.CONNECTING)
            {
                Console.WriteLine("等待连接完成...");
                Thread.Sleep(1000);
            }

            var state = zooKeeper.getState();
            if (state != ZooKeeper.States.CONNECTED && state != ZooKeeper.States.CONNECTEDREADONLY)
            {
                Console.WriteLine("连接失败：" + state);
                Console.ReadKey();
                return;
            }

            //创建znode节点
            {
                var data = Encoding.UTF8.GetBytes("hello world");
                List<ACL> acl = ZooDefs.Ids.OPEN_ACL_UNSAFE;//创建节点时的acl权限，也可以使用下面的自定义权限
                //List<ACL> acl = new List<ACL>() {
                //    new ACL((int)ZooDefs.Perms.READ, new Id("ip", "127.0.0.1")),
                //    new ACL((int)(ZooDefs.Perms.READ | ZooDefs.Perms.WRITE), new Id("auth", "id:pass"))
                //};
                CreateMode createMode = CreateMode.PERSISTENT;
                zooKeeper.createAsync("/mynode", data, acl, createMode).Wait();
                Console.WriteLine("完成创建节点");
            }

            //节点是否存在
            {
                var exists = zooKeeper.existsAsync("/mynode", new MyWatcher("ExistsWatcher")).GetAwaiter().GetResult();
                Console.WriteLine("节点是否存在：" + exists);
            }

            //获取节点数据
            {
                var dataResult = zooKeeper.getDataAsync("/mynode", new MyWatcher("GetWatcher")).GetAwaiter().GetResult();
                var value = Encoding.UTF8.GetString(dataResult.Data);
                Console.WriteLine("完成读取节点：" + value);
            }

            //设置节点数据
            {
                var data = Encoding.UTF8.GetBytes("hello world again");
                zooKeeper.setDataAsync("/mynode", data);
                Console.WriteLine("设置节点数据");
            }

            //重新获取节点数据
            {
                var dataResult = zooKeeper.getDataAsync("/mynode", new MyWatcher("GetWatcher")).GetAwaiter().GetResult();
                var value = Encoding.UTF8.GetString(dataResult.Data);
                Console.WriteLine("重新获取节点数据：" + value);
            }

            //移除节点
            {
                zooKeeper.deleteAsync("/mynode").Wait();
                Console.WriteLine("移除节点");
            }

            Console.WriteLine("完成");
            Console.ReadKey();
        }
    }
    class MyWatcher : Watcher
    {
        public string Name { get; private set; }

        public MyWatcher(string name)
        {
            this.Name = name;
        }

        public override Task process(WatchedEvent @event)
        {
            var path = @event.getPath();
            var state = @event.getState();

            Console.WriteLine($"{Name} recieve: Path-{path}     State-{@event.getState()}    Type-{@event.get_Type()}");
            return Task.FromResult(0);
        }
    }
}
