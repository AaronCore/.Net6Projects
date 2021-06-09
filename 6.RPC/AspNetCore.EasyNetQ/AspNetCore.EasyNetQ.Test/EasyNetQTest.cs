using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace AspNetCore.EasyNetQ.Test
{
    public class EasyNetQTest : BaseUnitTest
    {
        ITestOutputHelper Output;
        public EasyNetQTest(ITestOutputHelper testOutputHelper)
        {
            Output = testOutputHelper;
        }

        /// <summary>
        /// 发布订阅模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PubSubTest()
        {
            string name = "PubSubTest";
            int count1 = 0, count2 = 0;

            EasyNetQServer easyNetQServer = new EasyNetQServer();
            easyNetQServer.Register(services =>
            {
                services.AddEasyNetQProducer(name, options =>
                {
                    //options.ConnectionString = connectionString;
                    options.Hosts = hosts;
                    options.Port = port;
                    options.Password = password;
                    options.UserName = userName;
                    options.VirtualHost = virtualHost;

                    options.PersistentMessages = true;
                    options.Priority = 1;
                    options.Topic = $"{topic}.{name}";
                });

                services.AddEasyNetQConsumer(options =>
                {
                    //options.ConnectionString = connectionString;
                    options.Hosts = hosts;
                    options.Port = port;
                    options.Password = password;
                    options.UserName = userName;
                    options.VirtualHost = virtualHost;

                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queue = $"{queue}.{name}";
                    options.Topic = $"{topic}.{name}";
                    options.PrefetchCount = 1;
                    options.Priority = 2;
                })
                .AddSubscriber<Subscriber>($"{name}1", r =>
                {
                    count1++;
                    Output.WriteLine($"{name}1:" + r.Message);
                })
                .AddSubscriber<Subscriber>($"{name}2", r =>
                {
                    count2++;
                    Output.WriteLine($"{name}2:" + r.Message);
                });

            });
            await easyNetQServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = easyNetQServer.ServiceProvider;
            var factory = serviceProvider.GetService<IBusClientFactory>();
            var busClient = factory.Create(name);
            for (var i = 0; i < 10; i++)
            {
                await busClient.PublishAsync(new Subscriber() { Message = "message" + i });
            }

            BlockUntil(() => count1 + count2 >= 10, 3000);

            Assert.Equal(10, count1 + count2);

            await easyNetQServer.StopAsync();
        }
        /// <summary>
        /// 请求响应模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RpcTest()
        {
            string name = "RpcTest";

            EasyNetQServer easyNetQServer = new EasyNetQServer();
            easyNetQServer.Register(services =>
            {
                services.AddEasyNetQProducer(name, options =>
                {
                    //options.ConnectionString = connectionString;
                    options.Hosts = hosts;
                    options.Port = port;
                    options.Password = password;
                    options.UserName = userName;
                    options.VirtualHost = virtualHost;

                    options.PersistentMessages = true;
                    options.Priority = 3;
                    options.Queue = $"{queue}.{name}";
                });

                services.AddEasyNetQConsumer(options =>
                {
                    //options.ConnectionString = connectionString;
                    options.Hosts = hosts;
                    options.Port = port;
                    options.Password = password;
                    options.UserName = userName;
                    options.VirtualHost = virtualHost;

                    options.Durable = true;
                    options.Queue = $"{queue}.{name}";
                    options.PrefetchCount = 2;
                })
                .AddResponder<Requester, Responder>(request =>
                {
                    return new Responder() { Result = $"{name}:" + request.Data };
                });
            });
            await easyNetQServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = easyNetQServer.ServiceProvider;
            var factory = serviceProvider.GetService<IBusClientFactory>();
            var busClient = factory.Create(name);
            var response = await busClient.RequestAsync<Requester, Responder>(new Requester() { Data = "message" });

            Thread.Sleep(3000);//等待运行3秒

            Assert.NotNull(response);

            await easyNetQServer.StopAsync();
        }
        /// <summary>
        /// 发送接收模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendReceiveTest()
        {
            string name = "SendReceiveTest";
            int count1 = 0, count2 = 0;

            EasyNetQServer easyNetQServer = new EasyNetQServer();
            easyNetQServer.Register(services =>
            {
                services.AddEasyNetQProducer(name, options =>
                {
                    //options.ConnectionString = connectionString;
                    options.Hosts = hosts;
                    options.Port = port;
                    options.Password = password;
                    options.UserName = userName;
                    options.VirtualHost = virtualHost;

                    options.Priority = 4;
                    options.Queue = $"{queue}.{name}";
                });

                services.AddEasyNetQConsumer(options =>
                {
                    //options.ConnectionString = connectionString;
                    options.Hosts = hosts;
                    options.Port = port;
                    options.Password = password;
                    options.UserName = userName;
                    options.VirtualHost = virtualHost;

                    options.Priority = 5;
                    options.PrefetchCount = 5;
                    options.Exclusive = false;
                    options.Arguments = arguments;
                    options.Queue = $"{queue}.{name}";
                })
                .AddReceiver<Reciever1>(r =>
                {
                    count1++;
                    Output.WriteLine("Reciever1:" + r.Message);
                })
                .AddReceiver<Reciever2>(r =>
                {
                    count2++;
                    Output.WriteLine("Reciever2:" + r.Message);
                });
            });
            await easyNetQServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = easyNetQServer.ServiceProvider;
            var factory = serviceProvider.GetService<IBusClientFactory>();
            var busClient = factory.Create(name);
            for (var i = 0; i < 10; i++)
            {
                if (i % 3 == 0)
                {
                    await busClient.SendAsync(new Reciever1() { Message = "message" + i });
                }
                else
                {
                    await busClient.SendAsync(new Reciever2() { Message = "message" + i });
                }
            }

            BlockUntil(() => count1 + count2 >= 10, 3000);

            Assert.Equal(4, count1);
            Assert.Equal(6, count2);

            await easyNetQServer.StopAsync();
        }
    }
}
