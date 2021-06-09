using EasyNetQ;
using EasyNetQ.Topology;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.Common
{
    public class RabbitMQManage
    {
        private volatile static IBus bus = null;
        private static readonly object lockMq = new object();

        /// <summary>
        /// 创建服务总线
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IBus CreateEventBus()
        {
            var config = ConfigManage.GetInstance().AppSettings["MeessageService"];
            if (string.IsNullOrEmpty(config))
                throw new Exception("消息地址未配置");
            if (bus == null && !string.IsNullOrEmpty(config))
            {
                lock (lockMq)
                {
                    if (bus == null)
                    {
                        bus = RabbitHutch.CreateBus(config);
                    }
                }
            }
            return bus;
        }

        /// <summary>
        ///  消息同步投递
        /// </summary>
        /// <param name="listMsg"></param>
        /// <returns></returns>
        public static bool PublishMessage(PublishMessage message)
        {
            bool b = true;
            try
            {
                if (bus == null)
                    CreateEventBus();
                new SendMange().SendMessage(message, bus);
                b = true;
            }
            catch (Exception ex)
            {
                b = false;
            }
            return b;
        }

        /// <summary>
        /// 消息异步投递
        /// </summary>
        /// <param name="listMsg"></param>
        public static async Task PublishMessageAsync(PublishMessage message)
        {
            try
            {
                if (bus == null)
                    CreateEventBus();
                await new SendMange().SendMessageAsync(message, bus);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 消息订阅
        /// </summary>
        public static void Subscribe<TConsum>(MessageArgs args) where TConsum : IMessageConsume, new()
        {
            if (bus == null)
                CreateEventBus();
            if (string.IsNullOrEmpty(args.ExchangeName))
                return;
            Expression<Action<TConsum>> methodCall;
            IExchange ex = null;
            //判断推送模式
            if (args.SendEnum == SendEnum.推送模式)
            {
                ex = bus.Advanced.ExchangeDeclare(args.ExchangeName, ExchangeType.Direct);
            }
            if (args.SendEnum == SendEnum.订阅模式)
            {
                //广播订阅模式
                ex = bus.Advanced.ExchangeDeclare(args.ExchangeName, ExchangeType.Fanout);
            }
            if (args.SendEnum == SendEnum.主题路由模式)
            {
                //主题路由模式
                ex = bus.Advanced.ExchangeDeclare(args.ExchangeName, ExchangeType.Topic);
            }
            IQueue qu;
            if (string.IsNullOrEmpty(args.RabbitQueeName))
            {
                qu = bus.Advanced.QueueDeclare();
            }
            else
            {
                qu = bus.Advanced.QueueDeclare(args.RabbitQueeName);
            }
            bus.Advanced.Bind(ex, qu, args.RouteName);
            bus.Advanced.Consume(qu, (body, properties, info) => Task.Factory.StartNew(() =>
            {
                try
                {
                    lock (lockMq)
                    {
                        var message = Encoding.UTF8.GetString(body);
                        //处理消息
                        methodCall = job => job.Consume(message);
                        methodCall.Compile()(new TConsum());
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            })
            );
        }

        /// <summary>
        /// 释放服务总线
        /// </summary>
        public static void DisposeBus()
        {
            bus?.Dispose();
        }

    }
}
