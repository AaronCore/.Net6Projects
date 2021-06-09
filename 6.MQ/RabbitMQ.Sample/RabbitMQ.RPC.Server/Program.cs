using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace RabbitMQ.RPC.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server端...");

            //使用RabbitMQ封装好的RPC
            //var factory = new ConnectionFactory()
            //{ HostName = "127.0.0.1", UserName = "guest", Password = "guest" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare("rpc_queue_1", false, false, false, null);//创建一个rpc queue
            //    var rpc = new MySimpleRpcServer(new Subscription(channel, "rpc_queue_1"));
            //    Console.WriteLine("服务端启动成功");
            //    rpc.MainLoop();
            //    Console.ReadKey();
            //}

            //手动实现RPC
            var factory = new ConnectionFactory()
            { HostName = "127.0.0.1", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);

                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        int n = int.Parse(message);
                        Console.WriteLine(" [.] fib({0})", message);
                        response = fib(n).ToString();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadKey();
            }
        }
        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return fib(n - 1) + fib(n - 2);
        }
    }
    class MySimpleRpcServer : SimpleRpcServer
    {
        public MySimpleRpcServer(Subscription subscription) : base(subscription)
        {
        }
        /// <summary>
        /// 执行完成后进行回调
        /// </summary>
        public override byte[] HandleSimpleCall(bool isRedelivered, IBasicProperties requestProperties, byte[] body, out IBasicProperties replyProperties)
        {
            replyProperties = null;
            return Encoding.UTF8.GetBytes($"给{Encoding.UTF8.GetString(body)}发送短信成功");
        }
    }
}
