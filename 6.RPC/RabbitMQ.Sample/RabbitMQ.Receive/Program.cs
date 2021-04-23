using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("接收端...");
            Console.BackgroundColor = ConsoleColor.Red;

            var factory = new ConnectionFactory()
            { HostName = "127.0.0.1", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //1. 入门
                //channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                //var consumer = new EventingBasicConsumer(channel);
                //consumer.Received += (model, ea) =>
                //{
                //    var body = ea.Body;
                //    var message = Encoding.UTF8.GetString(body);
                //    Console.WriteLine("接收内容：{0}", message);
                //};
                //channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

                //2.0
                var exchangeName = "exchange.test";
                var queueName = "queue.test";
                var routeName = "route.test";
                //声明交换机
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
                //声明队列
                channel.QueueDeclare(queueName, true, false, false, null);
                //设置prefetchCount : 1来告知RabbitMQ，在未收到消费端的消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                //绑定队列
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routeName, null);
                //构造消费者实例
                var consumer = new EventingBasicConsumer(channel);
                //while (true)
                //{
                //    var response = channel.BasicGet(queueName, autoAck: false);
                //    if (response != null)
                //    {
                //        var msg = Encoding.UTF8.GetString(response.Body);
                //        Console.WriteLine(msg);
                //    }
                //}
                //绑定消息接收后的事件委托
                consumer.Received += (sender, e) =>
                {
                    var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine(msg);
                    Thread.Sleep(3000);//模拟耗时
                    //发送消息确认信号（手动消息确认）
                    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                };
                //启动消费
                //autoAck:true；自动进行消息确认，当消费端接收到消息后，就自动发送ack信号，不管消息是否正确处理完毕
                //autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认
                channel.BasicConsume(queueName, false, consumer);

                Console.ReadKey();
            }
        }
    }
}
