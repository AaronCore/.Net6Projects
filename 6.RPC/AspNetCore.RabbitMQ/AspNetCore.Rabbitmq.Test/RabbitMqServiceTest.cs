using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AspNetCore.RabbitMQ.Logger;
using System.Threading;
using Microsoft.Extensions.Logging;
using AspNetCore.RabbitMQ.Integration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit.Abstractions;
using System.Text.Json;

namespace AspNetCore.RabbitMQ.Test
{
    public class RabbitMqServiceTest : BaseUnitTest
    {
        ITestOutputHelper Output;
        public RabbitMqServiceTest(ITestOutputHelper testOutputHelper)
        {
            Output = testOutputHelper;
        }

        #region Logger
        private void WriteLogger(string queue, RecieveResult result)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(result.Body);
            var applicationName = dict[nameof(RabbitLoggerMessage<string>.ApplicationName)];
            var category = dict[nameof(RabbitLoggerMessage<string>.Category)];
            var logLevel = dict[nameof(RabbitLoggerMessage<string>.LogLevel)];
            var message = dict[nameof(RabbitLoggerMessage<string>.Message)];
            Output.WriteLine($@"【{queue}】{applicationName}:{category}-{logLevel}-{message}");
        }

        /// <summary>
        /// 发送到队列的日志测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoggerQueueTest()
        {
            int counter = 0;
            string queue = $"{logger}.queue";

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                });
                services.AddRabbitLogger(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queue = queue;
                    options.Type = RabbitExchangeType.None;
                    options.RouteQueues = null;
                    options.Exchange = string.Empty;

                    options.ApplicationName = nameof(RabbitMqServiceTest.LoggerQueueTest);
                    options.Category = nameof(RabbitMqServiceTest);
                    options.MinLevel = LogLevel.Trace;
                });

                services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.None;
                    options.RouteQueues = null;

                    options.FetchCount = 10;
                    options.AutoAck = true;
                }).AddListener(queue, result =>
                {
                    WriteLogger(queue, result);
                    if (result.Body.Contains(nameof(RabbitMqServiceTest)))
                    {
                        counter++;
                    }
                });
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var _logger = loggerFactory.CreateLogger<RabbitMqServiceTest>();

            _logger.LogTrace("LogTrace");
            _logger.LogDebug("LogDebug");
            _logger.LogInformation("LogInformation");
            _logger.LogWarning("LogWarning");
            _logger.LogError("LogError");
            _logger.LogCritical("LogCritical");

            BlockUntil(() => counter >= 6, 3000);

            Thread.Sleep(1000);//等待运行1秒
            Assert.Equal(6, counter);

            await rabbitServer.StopAsync();
        }
        /// <summary>
        /// 发送到Direct交换机的日志测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoggerDirectTest()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();//接收的消息数量
            Dictionary<string, int> expectedDict = new Dictionary<string, int>();//发送的消息数量
            string[] queues = new string[] { $"{logger}.{queue1}.direct", $"{logger}.{queue2}.direct" };
            var routeQueues = queues.Select(f => new RouteQueue()
            {
                Arguments = null,
                Queue = f,
                Route = $"{nameof(RabbitMqServiceTest)}.{f}",
                Options = new QueueOptions()
                {
                    AutoDelete = true,
                    Durable = true,
                    Arguments = arguments
                }
            }).ToArray();

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                });

                services.AddRabbitLogger(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queue = string.Empty;
                    options.Type = RabbitExchangeType.Direct;
                    options.RouteQueues = routeQueues;
                    options.Exchange = direct;

                    options.ApplicationName = nameof(RabbitMqServiceTest.LoggerDirectTest);
                    options.Category = nameof(RabbitMqServiceTest);
                    options.MinLevel = LogLevel.Information;
                });

                var builder = services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.Direct;
                    options.RouteQueues = routeQueues;

                    options.FetchCount = 10;
                    options.AutoAck = true;
                });
                foreach (var queue in queues)
                {
                    dict[queue] = 0;
                    expectedDict[queue] = 0;
                    builder.AddListener(direct, queue, result =>
                    {
                        WriteLogger(queue, result);
                        if (result.Body.Contains(nameof(RabbitMqServiceTest)))
                        {
                            dict[queue]++;
                        }
                    });
                }
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            int per = 4, count = 10;
            for (var i = 0; i < count; i++)
            {
                var routeQueue = routeQueues[new Random().Next(0, routeQueues.Length)];
                var logger = loggerFactory.CreateLogger(routeQueue.Route);

                logger.LogTrace("LogTrace");
                logger.LogDebug("LogDebug");
                logger.LogInformation("LogInformation");
                logger.LogWarning("LogWarning");
                logger.LogError("LogError");
                logger.LogCritical("LogCritical");

                expectedDict[routeQueue.Queue] += per;
            }

            BlockUntil(() => dict.Sum(f => f.Value) >= per * count, 3000);

            Thread.Sleep(1000);//等待运行1秒
            foreach (var queue in queues)
            {
                Output.WriteLine($"{queue}:{expectedDict[queue]}-{dict[queue]}");
                Assert.Equal(expectedDict[queue], dict[queue]);
            }

            await rabbitServer.StopAsync();
        }
        /// <summary>
        /// 发送到Fanout交换机的日志测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoggerFanoutTest()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();//接收的消息数量

            string[] queues = new string[] { $"{logger}.{queue1}.fanout", $"{logger}.{queue2}.fanout" };
            var routeQueues = queues.Select(f => new RouteQueue()
            {
                Arguments = null,
                Queue = f,
                Route = string.Empty,
                Options = new QueueOptions()
                {
                    AutoDelete = true,
                    Durable = true,
                    Arguments = arguments
                }
            }).ToArray();

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                });

                services.AddRabbitLogger(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queue = string.Empty;
                    options.Type = RabbitExchangeType.Fanout;
                    options.RouteQueues = routeQueues;
                    options.Exchange = fanout;

                    options.ApplicationName = nameof(RabbitMqServiceTest.LoggerFanoutTest);
                    options.Category = nameof(RabbitMqServiceTest);
                    options.MinLevel = LogLevel.Information;
                });

                var builder = services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.Fanout;
                    options.RouteQueues = routeQueues;

                    options.FetchCount = 10;
                    options.AutoAck = true;
                });
                foreach (var queue in queues)
                {
                    dict[queue] = 0;
                    builder.AddListener(fanout, queue, result =>
                    {
                        WriteLogger(queue, result);
                        if (result.Body.Contains(nameof(RabbitMqServiceTest)))
                        {
                            dict[queue]++;
                        }
                    });
                }
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var _logger = loggerFactory.CreateLogger($"{nameof(RabbitMqServiceTest)}");

            _logger.LogTrace("LogTrace");
            _logger.LogDebug("LogDebug");
            _logger.LogInformation("LogInformation");
            _logger.LogWarning("LogWarning");
            _logger.LogError("LogError");
            _logger.LogCritical("LogCritical");

            BlockUntil(() => dict.Sum(f => f.Value) >= 4 * 2, 3000);

            Thread.Sleep(1000);//等待运行1秒
            foreach (var queue in queues)
            {
                Output.WriteLine($"{queue}:{dict[queue]}");
                Assert.Equal(4, dict[queue]);
            }

            await rabbitServer.StopAsync();
        }
        /// <summary>
        /// 发送到Topic交换机的日志测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoggerTopicTest()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();//接收的消息数量
            Dictionary<string, int> expectedDict = new Dictionary<string, int>();//发送的消息数量
            string[] queues = new string[] { $"{logger}.{queue1}.topic", $"{logger}.{queue2}.topic" };
            var routeQueues = queues.Select(f => new RouteQueue()
            {
                Arguments = null,
                Queue = f,
                Route = $"#.{f}.#",
                Options = new QueueOptions()
                {
                    AutoDelete = true,
                    Durable = true,
                    Arguments = arguments
                }
            }).ToArray();

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                });

                services.AddRabbitLogger(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queue = string.Empty;
                    options.Type = RabbitExchangeType.Topic;
                    options.RouteQueues = routeQueues;
                    options.Exchange = topic;

                    options.ApplicationName = nameof(RabbitMqServiceTest.LoggerTopicTest);
                    options.Category = nameof(RabbitMqServiceTest);
                    options.MinLevel = LogLevel.Information;
                });

                var builder = services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.Topic;
                    options.RouteQueues = routeQueues;

                    options.FetchCount = 10;
                    options.AutoAck = true;
                });
                foreach (var queue in queues)
                {
                    dict[queue] = 0;
                    expectedDict[queue] = 0;
                    builder.AddListener(topic, queue, result =>
                    {
                        WriteLogger(queue, result);
                        if (result.Body.Contains(nameof(RabbitMqServiceTest)))
                        {
                            dict[queue]++;
                        }
                    });
                }
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            int per = 4, count = 10;
            for (var i = 0; i < count; i++)
            {
                var routeQueue = routeQueues[new Random().Next(0, routeQueues.Length)];
                var logger = loggerFactory.CreateLogger($"{nameof(RabbitMqServiceTest)}.{routeQueue.Queue}.{i}");

                logger.LogTrace("LogTrace");
                logger.LogDebug("LogDebug");
                logger.LogInformation("LogInformation");
                logger.LogWarning("LogWarning");
                logger.LogError("LogError");
                logger.LogCritical("LogCritical");

                expectedDict[routeQueue.Queue] += per;
            }

            BlockUntil(() => dict.Sum(f => f.Value) >= per * count, 3000);

            Thread.Sleep(1000);//等待运行1秒
            foreach (var queue in queues)
            {
                Output.WriteLine($"{queue}:{expectedDict[queue]}-{dict[queue]}");
                Assert.Equal(expectedDict[queue], dict[queue]);
            }

            await rabbitServer.StopAsync();
        }
        #endregion
        /// <summary>
        /// 简单模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimplePatternTest()
        {
            int counter = 0;
            string queue = $"{queue1}.simple";

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddRabbitProducer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queues = new string[] { queue };
                    options.Type = RabbitExchangeType.None;
                    options.RouteQueues = null;
                    options.Exchange = string.Empty;

                    options.InitializeCount = 10;
                });

                services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.None;
                    options.RouteQueues = null;

                    options.FetchCount = 10;
                    options.AutoAck = true;
                }).AddListener(queue, result =>
                {
                    Output.WriteLine(JsonSerializer.Serialize(result));
                    counter++;
                });
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var factory = serviceProvider.GetService<IRabbitProducerFactory>();
            var producer = factory.Create();
            await producer.PublishAsync("hello simple");
            await producer.PublishAsync("hello simple again");

            BlockUntil(() => counter >= 2, 3000);

            Assert.Equal(2, counter);

            await rabbitServer.StopAsync();
        }
        /// <summary>
        /// 工作模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task WorkerPatternTest()
        {
            int counter1 = 0, counter2 = 0;
            string queue = $"{queue1}.worker";

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddRabbitProducer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queues = new string[] { queue };
                    options.Type = RabbitExchangeType.None;
                    options.RouteQueues = null;
                    options.Exchange = string.Empty;

                    options.InitializeCount = 10;
                });

                services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.None;
                    options.RouteQueues = null;

                    options.FetchCount = 1;
                    options.AutoAck = false;
                }).AddListener(queue, result =>
                {
                    Output.WriteLine($"worker1:" + JsonSerializer.Serialize(result));
                    counter1++;
                    result.Commit();
                });

                services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.None;
                    options.RouteQueues = null;

                    options.FetchCount = 2;
                    options.AutoAck = false;
                }).AddListener(queue, result =>
                {
                    Output.WriteLine($"worker2:" + JsonSerializer.Serialize(result));
                    counter2++;
                    result.Commit();
                });
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var factory = serviceProvider.GetService<IRabbitProducerFactory>();
            var producer = factory.Create();
            for (var i = 0; i < 10; i++)
            {
                await producer.PublishAsync("worker" + i);
            }

            BlockUntil(() => counter1 + counter2 >= 10, 3000);

            //单元测试状态下QOS貌似不生效
            Output.WriteLine($"worker1:{counter1}   worker2:{counter2}");
            Assert.True(counter1 <= counter2);

            await rabbitServer.StopAsync();
        }
        /// <summary>
        /// 路由模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DirectPatternTest()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();//接收的消息数量
            Dictionary<string, int> expectedDict = new Dictionary<string, int>();//发送的消息数量
            string[] queues = new string[] { $"{queue1}.direct", $"{queue2}.direct" };
            var routeQueues = queues.Select(f => new RouteQueue()
            {
                Arguments = null,
                Queue = f,
                Route = $"{nameof(RabbitMqServiceTest)}.{f}",
                Options = new QueueOptions()
                {
                    AutoDelete = true,
                    Durable = true,
                    Arguments = arguments
                }
            }).ToArray();

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddRabbitProducer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queues = null;
                    options.Type = RabbitExchangeType.Direct;
                    options.RouteQueues = routeQueues;
                    options.Exchange = direct;

                    options.InitializeCount = 10;
                });

                var builder = services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.Direct;
                    options.RouteQueues = routeQueues;

                    options.FetchCount = 1;
                    options.AutoAck = false;
                });
                foreach (var queue in queues)
                {
                    dict[queue] = 0;
                    expectedDict[queue] = 0;
                    builder.AddListener(direct, queue, result =>
                    {
                        Output.WriteLine($"{queue}:" + JsonSerializer.Serialize(result));
                        dict[queue]++;
                        result.Commit();
                    });
                }
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var factory = serviceProvider.GetService<IRabbitProducerFactory>();
            var producer = factory.Create();
            for (var i = 0; i < 10; i++)
            {
                var routeQueue = routeQueues[new Random().Next(0, routeQueues.Length)];
                await producer.PublishAsync(routeQueue.Route, "direct" + i);
                expectedDict[routeQueue.Queue] += 1;
            }

            BlockUntil(() => dict.Sum(f => f.Value) >= 10, 3000);

            foreach (var queue in queues)
            {
                Output.WriteLine($"{queue}:{expectedDict[queue]}-{dict[queue]}");
                Assert.Equal(expectedDict[queue], dict[queue]);
            }

            await rabbitServer.StopAsync();
        }
        /// <summary>
        /// 发布订阅模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FanoutPatternTest()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();//接收的消息数量

            string[] queues = new string[] { $"{queue1}.fanout", $"{queue2}.fanout" };
            var routeQueues = queues.Select(f => new RouteQueue()
            {
                Arguments = null,
                Queue = f,
                Route = string.Empty,
                Options = new QueueOptions()
                {
                    AutoDelete = true,
                    Durable = true,
                    Arguments = arguments
                }
            }).ToArray();

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddRabbitProducer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queues = null;
                    options.Type = RabbitExchangeType.Fanout;
                    options.RouteQueues = routeQueues;
                    options.Exchange = fanout;

                    options.InitializeCount = 10;
                });

                var builder = services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.Fanout;
                    options.RouteQueues = routeQueues;

                    options.FetchCount = 1;
                    options.AutoAck = false;
                });
                foreach (var queue in queues)
                {
                    dict[queue] = 0;
                    builder.AddListener(fanout, queue, result =>
                    {
                        Output.WriteLine($"{queue}:" + JsonSerializer.Serialize(result));
                        dict[queue]++;
                        result.Commit();
                    });
                }
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var factory = serviceProvider.GetService<IRabbitProducerFactory>();
            var producer = factory.Create();
            for (var i = 0; i < 10; i++)
            {
                await producer.PublishAsync(string.Empty, "fanout" + i);
            }

            BlockUntil(() => dict.Sum(f => f.Value) >= queues.Length * 10, 3000);

            foreach (var queue in queues)
            {
                Output.WriteLine($"{queue}:{dict[queue]}");
                Assert.Equal(10, dict[queue]);
            }

            await rabbitServer.StopAsync();
        }
        /// <summary>
        /// Topic模式
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TopicPatternTest()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();//接收的消息数量
            Dictionary<string, int> expectedDict = new Dictionary<string, int>();//发送的消息数量
            string[] queues = new string[] { $"{queue1}.topic", $"{queue2}.topic" };
            var routeQueues = queues.Select(f => new RouteQueue()
            {
                Arguments = null,
                Queue = f,
                Route = $"#.{f}.#",
                Options = new QueueOptions()
                {
                    AutoDelete = true,
                    Durable = true,
                    Arguments = arguments
                }
            }).ToArray();

            RabbitServer rabbitServer = new RabbitServer();
            rabbitServer.Register(services =>
            {
                services.AddRabbitProducer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Queues = null;
                    options.Type = RabbitExchangeType.Topic;
                    options.RouteQueues = routeQueues;
                    options.Exchange = topic;

                    options.InitializeCount = 10;
                });

                var builder = services.AddRabbitConsumer(options =>
                {
                    options.Hosts = hosts;
                    options.Port = port;
                    options.UserName = userName;
                    options.Password = password;
                    options.VirtualHost = virtualHost;

                    options.Arguments = arguments;
                    options.AutoDelete = true;
                    options.Durable = true;
                    options.Type = RabbitExchangeType.Topic;
                    options.RouteQueues = routeQueues;

                    options.FetchCount = 1;
                    options.AutoAck = false;
                });
                foreach (var queue in queues)
                {
                    dict[queue] = 0;
                    expectedDict[queue] = 0;
                    builder.AddListener(topic, queue, result =>
                    {
                        Output.WriteLine($"{queue}:" + JsonSerializer.Serialize(result));
                        dict[queue]++;
                        result.Commit();
                    });
                }
            });
            await rabbitServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = rabbitServer.ServiceProvider;
            var factory = serviceProvider.GetService<IRabbitProducerFactory>();
            var producer = factory.Create();
            for (var i = 0; i < 10; i++)
            {
                var routeQueue = routeQueues[new Random().Next(0, routeQueues.Length)];
                await producer.PublishAsync($"{nameof(RabbitMqServiceTest)}.{routeQueue.Queue}.{i}", "direct" + i);
                expectedDict[routeQueue.Queue] += 1;
            }

            BlockUntil(() => dict.Sum(f => f.Value) >= 10, 3000);

            foreach (var queue in queues)
            {
                Output.WriteLine($"{queue}:{expectedDict[queue]}-{dict[queue]}");
                Assert.Equal(expectedDict[queue], dict[queue]);
            }

            await rabbitServer.StopAsync();
        }
    }
}
