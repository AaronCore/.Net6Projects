using System;
using EasyNetQ.Common;

namespace EasyNetQ.Send
{
    /*
      *文档：
      * https://github.com/EasyNetQ/EasyNetQ/wiki/Quick-Start
      */
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //需要注意一点儿，如果发送的时候，在该管道下找不到相匹配的队列框架将默认丢弃该消息
                Console.WriteLine("Start......");

                //推送模式
                //推送模式下，需指定管道名称和路由键值名称
                //消息只会被发送到指定的队列中去
                var direct = new PublishMessage()
                {
                    SendMessage = new Test()
                    {
                        Var1 = "这是推送模式"
                    },
                    ExchangeName = "message.directdemo",
                    RouteName = "routekey",
                    SendEnum = SendEnum.推送模式
                };
                //同步发送 ，返回true或fasle true 发送成功，消息已存储到Rabbitmq中，false表示发送失败
                var b = RabbitMQManage.PublishMessage(direct);
                //异步发送，如果失败，失败的消息会被写入数据库，会有后台线程轮询数据库进行重新发送
                //RabbitMQManage.PublishMessageAsync(directdto);


                //订阅模式
                //订阅模式只需要指定管道名称
                //消息会被发送到该管道下的所有队列中
                var fanout = new PublishMessage()
                {
                    SendMessage = new Test()
                    {
                        Var1 = "这是订阅模式"
                    },
                    ExchangeName = "message.fanoutdemo",
                    RouteName = "routekey1",
                    SendEnum = SendEnum.订阅模式
                };
                //同步发送
                var fb = RabbitMQManage.PublishMessage(fanout);
                //异步发送
                // RabbitMQManage.PublishMessageAsync(fanoutdto);


                //主题路由模式
                //路由模式下需指定 管道名称和路由值
                //消息会被发送到该管道下，和路由值匹配的队列中去
                var route = new PublishMessage()
                {
                    SendMessage = new Test()
                    {
                        Var1 = "这是主题路由模式1",
                    },
                    ExchangeName = "message.topicdemo",
                    RouteName = "a.log",
                    SendEnum = SendEnum.主题路由模式
                };
                var route2 = new PublishMessage()
                {
                    SendMessage = new Test()
                    {
                        Var1 = "这是主题路由模式2",
                    },
                    ExchangeName = "message.topicdemo",
                    RouteName = "a.log.a.b",
                    SendEnum = SendEnum.主题路由模式
                };

                //同步发送
                var rb = RabbitMQManage.PublishMessage(route);
                var rb2 = RabbitMQManage.PublishMessage(route2);
                //异步发送
                //RabbitMQManage.PublishMessageAsync(route);
                //RabbitMQManage.PublishMessageAsync(route2);

                Console.WriteLine("End......");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error......");
                Console.WriteLine(ex.ToString());
            }
        }
    }
    /// <summary>
    /// 发送的数据
    /// </summary>
    public class Test
    {
        public string Var1 { get; set; }
        public string Var2 { get; set; }
        public string Var3 { get; set; }
    }
}
