using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Send
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("发送端...");

            var factory = new ConnectionFactory()
            { HostName = "127.0.0.1", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //1. 入门
                //channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                //var input = "";
                //Console.WriteLine("Enter a message. 'exit' to quit.");
                //while ((input = Console.ReadLine()) != "exit")
                //{
                //    var body = Encoding.UTF8.GetBytes(input);
                //    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                //    Console.WriteLine("发送内容：{0}", input);
                //}

                //2.0
                var exchangeName = "exchange.test";
                var queueName = "queue.test";
                var routeName = "route.test";
                //声明交换机
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
                //申明队列(指定durable:true,告知rabbitmq对消息进行持久化)
                channel.QueueDeclare(queueName, true, false, false, null);

                //绑定队列
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routeName, null);
                //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                for (int i = 1; i <= 100; i++)
                {
                    var body = Encoding.UTF8.GetBytes($"{i}：Hello RabbitMQ");
                    //发送数据包(指定basicProperties)
                    channel.BasicPublish(exchange: exchangeName, routingKey: routeName, basicProperties: properties, body: body);
                    Console.WriteLine("发送内容：{0}", $"{i}：Hello RabbitMQ");
                }
            }
            Console.ReadKey();
        }
    }
}
