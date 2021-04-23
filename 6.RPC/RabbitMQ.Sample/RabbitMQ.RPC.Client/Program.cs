using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.RPC.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client端...");

            //使用RabbitMQ封装好的RPC
            //var factory = new ConnectionFactory()
            //{ HostName = "127.0.0.1", UserName = "guest", Password = "guest" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    //创建client rpc
            //    var client = new SimpleRpcClient(channel, new PublicationAddress(exchangeType: ExchangeType.Direct, exchangeName: string.Empty, routingKey: "rpc_queue_1"));
            //    bool flag = true;
            //    var sendmsg = "";
            //    while (flag)
            //    {
            //        Console.WriteLine("请输入要发送的消息");
            //        sendmsg = Console.ReadLine();
            //        if (string.IsNullOrWhiteSpace(sendmsg))
            //        {
            //            Console.Write("请输入消息");
            //            continue;
            //        }
            //        var msg = client.Call(Encoding.UTF8.GetBytes(sendmsg));
            //        Console.WriteLine(Encoding.UTF8.GetString(msg));
            //    }
            //    Console.ReadKey();
            //}

            //手动实现RPC
            string n = args.Length > 0 ? args[0] : "20";
            Task t = InvokeAsync(n);
            t.Wait();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadKey();
        }
        private static async Task InvokeAsync(string n)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var rpcClient = new RpcClient();

            Console.WriteLine(" [x] Requesting fib({0})", n);
            var response = await rpcClient.CallAsync(n.ToString());
            Console.WriteLine(" [.] Got '{0}'", response);

            rpcClient.Close();
        }
    }
    public class RpcClient
    {
        private const string QUEUE_NAME = "rpc_queue";

        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper =
                    new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        public RpcClient()
        {
            var factory = new ConnectionFactory()
            { HostName = "127.0.0.1", UserName = "guest", Password = "guest" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<string> tcs))
                    return;
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                tcs.TrySetResult(response);
            };
        }
        public Task<string> CallAsync(string message, CancellationToken cancellationToken = default(CancellationToken))
        {
            IBasicProperties props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var tcs = new TaskCompletionSource<string>();
            callbackMapper.TryAdd(correlationId, tcs);

            channel.BasicPublish(
                exchange: "",
                routingKey: QUEUE_NAME,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out var tmp));
            return tcs.Task;
        }
        public void Close()
        {
            connection.Close();
        }
    }
}
