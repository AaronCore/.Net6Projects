using AspNetCore.ActiveMQ.Integration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCore.ActiveMQ.Test
{
    public class ActiveMqIntegrationTest : BaseUnitTest
    {
        ITestOutputHelper Output;
        public ActiveMqIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            Output = testOutputHelper;
        }


        /// <summary>
        /// 队列集成测试
        /// </summary>
        [Fact]
        public async Task QueueTest()
        {
            int counter = 0;
            var destination = $"integration.{queue}";

            //消费
            var consumer = new ActiveConsumer(true, brokerUris)
            {
                Password = password,
                UserName = userName
            };
            await consumer.ListenAsync(new ListenOptions()
            {
                AutoAcknowledge = false,
                Destination = destination,
                Durable = false,
                FromQueue = true,
                PrefetchCount = 10,
                RecoverWhenNotAcknowledge = false
            }, result =>
            {
                Output.WriteLine($"{destination}:" + result.Message);
                counter++;
                result.Commit();
            });

            //发布
            var producer = new ActiveProducer(true, brokerUris)
            {
                Password = password,
                UserName = userName
            };
            await producer.SendAsync(destination, "hello active");

            BlockUntil(() => counter >= 1, 3000);

            producer.Dispose();
            consumer.Dispose();

            Assert.Equal(1, counter);
        }
        /// <summary>
        /// Topic集成测试
        /// </summary>
        [Fact]
        public async Task TopicTest()
        {
            int counter = 0;
            var destination = $"integration.{topic}";

            //消费
            var consumer = new ActiveConsumer(true, brokerUris)
            {
                Password = password,
                UserName = userName
            };
            await consumer.ListenAsync(new ListenOptions()
            {
                AutoAcknowledge = false,
                Destination = destination,
                Durable = false,
                FromQueue = false,
                PrefetchCount = 10,
                RecoverWhenNotAcknowledge = false
            }, result =>
            {
                Output.WriteLine($"{destination}:" + result.Message);
                counter++;
                result.Commit();
            });

            //发布
            var producer = new ActiveProducer(true, brokerUris)
            {
                Password = password,
                UserName = userName
            };
            await producer.PublishAsync(destination, "hello active");

            BlockUntil(() => counter >= 1, 3000);

            producer.Dispose();
            consumer.Dispose();

            Assert.Equal(1, counter);
        }
        /// <summary>
        /// selector测试
        /// </summary>
        [Fact]
        public async Task SelectorTest()
        {
            int counter = 0;
            var destination = $"integration.selector.{queue}";


            /*
             * 数字表达式： >,>=,<,<=,BETWEEN,=
             * 字符表达式：=,<>,IN
             * IS NULL 或 IS NOT NULL
             * 逻辑AND, 逻辑OR, 逻辑NOT
             *
             * 常数类型
             * 数字：3.1415926， 5
             * 字符： ‘a’，必须带有单引号
             * NULL，特别的常量
             * 布尔类型： TRUE，FALSE
             */

            //消费
            var consumer = new ActiveConsumer(true, brokerUris)
            {
                Password = password,
                UserName = userName
            };
            await consumer.ListenAsync(new ListenOptions()
            {
                AutoAcknowledge = false,
                Destination = destination,
                Durable = false,
                FromQueue = true,
                PrefetchCount = 10,
                RecoverWhenNotAcknowledge = false,
                Selector = "apple = 10000 and xiaomi in ('1000','2000','3000') and huawei is null"
            }, result =>
            {
                Output.WriteLine($"{destination}:" + result.Message);
                counter++;
                result.Commit();
            });

            //发布
            var producer = new ActiveProducer(true, brokerUris)
            {
                Password = password,
                UserName = userName
            };
            var dict = new Dictionary<string, object>()
            {
                { "apple", 10000 },
                { "xiaomi", "5000" }
            };
            await producer.SendAsync(new ActiveMessage()
            {
                Destination = destination,
                Message = "hello active",
                Properties = dict
            });

            dict["xiaomi"] = "2000";
            await producer.SendAsync(new ActiveMessage()
            {
                Destination = destination,
                Message = "hello active again",
                Properties = dict
            });

            BlockUntil(() => counter >= 1, 3000);

            producer.Dispose();
            consumer.Dispose();

            Assert.Equal(1, counter);
        }
    }
}
