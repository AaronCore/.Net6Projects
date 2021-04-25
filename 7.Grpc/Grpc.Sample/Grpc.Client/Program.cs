using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Server;
using GrpcService;

namespace Grpc.Client
{
    /*
     * 参考资料：
     * https://docs.microsoft.com/zh-cn/aspnet/core/grpc/?view=aspnetcore-3.0
     * https://www.cnblogs.com/stulzq/category/1555261.html
     *
     */
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var invoker = channel.Intercept(new ClientLoggerInterceptor());

            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "欢迎光临..." });
            Console.WriteLine("Greeter 服务返回数据: " + reply.Message);

            var speakClient = new Speak.SpeakClient(channel);
            var speakReply = await speakClient.SpeakLAsync(new Empty());
            Console.WriteLine("调用服务：" + speakReply.Message);


            var chatClient = new Speak.SpeakClient(channel);
            //获取语言总数
            var chatCount = await chatClient.CountAsync(new Empty());
            Console.WriteLine($"语言总计：{chatCount.Count}。");
            var rand = new Random(DateTime.Now.Millisecond);


            //var chat = chatClient.Chat();

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));//5s后主动取消控制
            var chat = chatClient.Chat(cancellationToken: cts.Token);

            //定义接收吸猫响应逻辑
            var chatRespTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var resp in chat.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine(resp.Message);
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Stream cancelled.");
                }
            });
            for (int i = 0; i < 10; i++)
            {
                await chat.RequestStream.WriteAsync(new ChatRequest() { Id = rand.Next(0, chatCount.Count) });
            }
            //发送完毕
            await chat.RequestStream.CompleteAsync();
            Console.WriteLine("客户端已发送完毕。");
            Console.WriteLine("接收结果：");
            //开始接收响应
            await chatRespTask;

            Console.ReadKey();
        }
    }
}
