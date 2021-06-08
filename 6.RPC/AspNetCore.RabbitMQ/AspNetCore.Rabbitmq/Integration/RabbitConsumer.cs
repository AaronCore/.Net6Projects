using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.RabbitMQ.Integration
{
    public class RabbitConsumer : RabbitBase
    {
        public RabbitConsumer(params string[] hostAndPorts) : base(hostAndPorts)
        {

        }

        /// <summary>
        /// 开始消费
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queue"></param>
        /// <param name="autoAck"></param>
        /// <param name="fetchCount"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        private ListenResult ConsumeInternal(IModel channel, string queue, bool autoAck, ushort? fetchCount, Action<RecieveResult> received)
        {
            if (fetchCount != null)
            {
                channel.BasicQos(0, fetchCount.Value, true);
            }
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            if (received != null)
            {
                consumer.Received += (sender, e) =>
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    using (var result = new RecieveResult(e, cancellationTokenSource))
                    {
                        if (!autoAck)
                        {
                            cancellationTokenSource.Token.Register(r =>
                            {
                                var recieveResult = r as RecieveResult;
                                if (recieveResult.IsCommit)
                                {
                                    channel.BasicAck(e.DeliveryTag, false);
                                }
                                else
                                {
                                    channel.BasicNack(e.DeliveryTag, false, recieveResult.Requeue);
                                }
                            }, result);
                        }
                        try
                        {
                            received.Invoke(result);
                        }
                        catch
                        {
                            if (!autoAck)
                            {
                                result.RollBack();
                            }
                        }
                    }

                };
            }

            channel.BasicConsume(queue, autoAck, consumer);
            ListenResult listenResult = new ListenResult();
            listenResult.Token.Register(() =>
            {
                try
                {
                    channel.Close();
                    channel.Dispose();
                }
                catch { }
            });
            return listenResult;
        }

        #region 普通模式、Work模式
        /// <summary>
        /// 从队列消费消息
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        public ListenResult Listen(string queue, ConsumeQueueOptions options = null, Action<RecieveResult> received = null)
        {
            options = options ?? new ConsumeQueueOptions();
            var channel = GetChannel();
            PrepareQueueChannel(channel, queue, options);

            return ConsumeInternal(channel, queue, options.AutoAck, options.FetchCount, received);
        }
        /// <summary>
        /// 从队列消费消息
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="configure"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        public ListenResult Listen(string queue, Action<ConsumeQueueOptions> configure, Action<RecieveResult> received = null)
        {
            ConsumeQueueOptions options = new ConsumeQueueOptions();
            configure?.Invoke(options);
            return Listen(queue, options, received);
        }
        #endregion
        #region 订阅模式、路由模式、Topic模式
        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        public ListenResult Listen(string exchange, string queue, ExchangeConsumeQueueOptions options = null, Action<RecieveResult> received = null)
        {
            if (string.IsNullOrEmpty(exchange))
            {
                throw new ArgumentException("exchange cannot be empty", nameof(exchange));
            }
            if (options.Type == RabbitExchangeType.None)
            {
                throw new NotSupportedException($"{nameof(RabbitExchangeType)} must be specified");
            }

            options = options ?? new ExchangeConsumeQueueOptions();
            var channel = GetChannel();
            PrepareExchangeChannel(channel, exchange, options);

            return ConsumeInternal(channel, queue, options.AutoAck, options.FetchCount, received);
        }
        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="configure"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        public ListenResult Listen(string exchange, string queue, Action<ExchangeConsumeQueueOptions> configure, Action<RecieveResult> received = null)
        {
            ExchangeConsumeQueueOptions options = new ExchangeConsumeQueueOptions();
            configure?.Invoke(options);
            return Listen(exchange, queue, options, received);
        }
        #endregion

        public static RabbitConsumer Create(RabbitBaseOptions rabbitBaseOptions)
        {
            var consumer = new RabbitConsumer(rabbitBaseOptions.Hosts);
            consumer.Password = rabbitBaseOptions.Password;
            consumer.Port = rabbitBaseOptions.Port;
            consumer.UserName = rabbitBaseOptions.UserName;
            consumer.VirtualHost = rabbitBaseOptions.VirtualHost;
            return consumer;
        }
    }
    public class RecieveResult : IDisposable
    {
        CancellationTokenSource cancellationTokenSource;
        public RecieveResult(BasicDeliverEventArgs arg, CancellationTokenSource cancellationTokenSource)
        {
            this.Body = Encoding.UTF8.GetString(arg.Body.ToArray());
            this.ConsumerTag = arg.ConsumerTag;
            this.DeliveryTag = arg.DeliveryTag;
            this.Exchange = arg.Exchange;
            this.Redelivered = arg.Redelivered;
            this.RoutingKey = arg.RoutingKey;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        /// <summary>
        /// 消息体
        /// </summary>
        public string Body { get; private set; }
        /// <summary>
        /// 消费者标签
        /// </summary>
        public string ConsumerTag { get; private set; }
        /// <summary>
        /// Ack标签
        /// </summary>
        public ulong DeliveryTag { get; private set; }
        /// <summary>
        /// 交换机
        /// </summary>
        public string Exchange { get; private set; }
        /// <summary>
        /// 是否Ack
        /// </summary>
        public bool Redelivered { get; private set; }
        /// <summary>
        /// 路由
        /// </summary>
        public string RoutingKey { get; private set; }
        /// <summary>
        /// 是否提交
        /// </summary>
        public bool IsCommit { get; private set; }
        /// <summary>
        /// 提交失败是否重新进入队列
        /// </summary>
        public bool Requeue { get; private set; }

        private void CancelNotify()
        {
            if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested) return;

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        /// <summary>
        /// 提交消息
        /// </summary>
        public void Commit()
        {
            if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested) return;

            IsCommit = true;
            CancelNotify();
        }
        /// <summary>
        /// 回滚消息
        /// </summary>
        /// <param name="requeue">是否重新进入队列</param>
        public void RollBack(bool requeue = false)
        {
            if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested) return;

            Requeue = requeue;
            IsCommit = false;
            (this as IDisposable).Dispose();
        }

        void IDisposable.Dispose()
        {
            CancelNotify();
        }
    }
    public class ListenResult
    {
        CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// CancellationToken
        /// </summary>
        public CancellationToken Token { get { return cancellationTokenSource.Token; } }
        /// <summary>
        /// 是否已停止
        /// </summary>
        public bool Stoped { get { return cancellationTokenSource.IsCancellationRequested; } }

        public ListenResult()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
