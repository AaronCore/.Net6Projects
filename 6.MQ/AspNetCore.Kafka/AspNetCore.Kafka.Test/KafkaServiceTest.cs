using AspNetCore.Kafka.Integration;
using AspNetCore.Kafka.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCore.Kafka.Test
{
    public class KafkaServiceTest : BaseUnitTest
    {
        ITestOutputHelper Output;
        public KafkaServiceTest(ITestOutputHelper testOutputHelper)
        {
            Output = testOutputHelper;
        }

        private void WriteLogger(RecieveResult result)
        {
            var topic = result.Topic;
            var key = result.Key;
            var partition = result.Partition;
            var offset = result.Offset;
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(result.Message);
            var applicationName = dict[nameof(KafkaLoggerMessage<string>.ApplicationName)];
            var category = dict[nameof(KafkaLoggerMessage<string>.Category)];
            var logLevel = dict[nameof(KafkaLoggerMessage<string>.LogLevel)];
            var message = dict[nameof(KafkaLoggerMessage<string>.Message)];
            Output.WriteLine($@"【{topic}({partition}-{offset}:{key})】{applicationName}:{category}-{logLevel}-{message}");
        }
        /// <summary>
        /// 日志测试
        /// </summary>
        [Fact]
        public async Task LoggerTest()
        {
            int counter = 0;
            KafkaServer kafkaServer = new KafkaServer();
            kafkaServer.Register(services =>
            {
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                });
                services.AddKafkaLogger(options =>
                {
                    options.BootstrapServers = hosts;
                    options.Category = nameof(KafkaServiceTest);
                    options.InitializeCount = 10;
                    //options.Key = nameof(KafkaServiceTest);
                    options.MinLevel = LogLevel.Trace;
                    options.Topic = $"service.{logger}";
                    options.ApplicationName = nameof(KafkaServiceTest);
                });

                services.AddKafkaConsumer(options =>
                {
                    options.BootstrapServers = hosts;
                    options.EnableAutoCommit = true;
                    options.GroupId = $"service.{group}.logger";
                    options.Subscribers = KafkaSubscriber.From($"service.{logger}");
                }).AddListener(result =>
                {
                    WriteLogger(result);
                    if (result.Message.Contains(nameof(KafkaServiceTest)))
                    {
                        counter++;
                    }
                });
            });
            await kafkaServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = kafkaServer.ServiceProvider;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var _logger = loggerFactory.CreateLogger<KafkaServiceTest>();

            _logger.LogTrace("LogTrace");
            _logger.LogDebug("LogDebug");
            _logger.LogInformation("LogInformation");
            _logger.LogWarning("LogWarning");
            _logger.LogError("LogError");
            _logger.LogCritical("LogCritical");

            BlockUntil(() => counter >= 6, 3000);

            Thread.Sleep(3000);//等待运行3秒
            Assert.Equal(6, counter);

            await kafkaServer.StopAsync();
        }
        /// <summary>
        /// 服务集成测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ServiceTest()
        {
            int counter = 0;
            KafkaServer kafkaServer = new KafkaServer();
            kafkaServer.Register(services =>
            {
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                });
                services.AddKafkaProducer(options =>
                {
                    options.BootstrapServers = hosts;
                    options.InitializeCount = 3;
                    options.Key = "kafka";
                    options.Topic = $"service.{topic}";
                });

                services.AddKafkaConsumer(options =>
                {
                    options.BootstrapServers = hosts;
                    options.EnableAutoCommit = false;
                    options.GroupId = $"service.{group}.kafka";
                    options.Subscribers = KafkaSubscriber.From($"service.{topic}");
                }).AddListener(result =>
                {
                    Output.WriteLine(JsonSerializer.Serialize(result));
                    counter++;
                    result.Commit();
                });
            });
            await kafkaServer.StartAsync();

            Thread.Sleep(1000);//等待运行1秒

            var serviceProvider = kafkaServer.ServiceProvider;
            var factory = serviceProvider.GetService<IKafkaProducerFactory>();
            var producer = factory.Create();

            for (var i = 0; i < 10; i++)
            {
                await producer.PublishAsync($"{nameof(KafkaServiceTest)}{i}", "message" + i);
            }

            BlockUntil(() => counter >= 10, 3000);

            Thread.Sleep(3000);//等待运行3秒
            Assert.Equal(10, counter);

            await kafkaServer.StopAsync();
        }
    }
}
