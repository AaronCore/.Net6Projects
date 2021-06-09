using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace AspNetCore.ActiveMQ.Integration
{
    public class ActiveConsumer : ActiveBase
    {
        public ActiveConsumer(bool isCluster, string[] hostAndPorts, string query = null) :
            base(isCluster, hostAndPorts, query)
        { }

        #region Private
        /// <summary>
        /// 获得一个连接
        /// </summary>
        /// <param name="listenOptions"></param>
        /// <returns></returns>
        private IConnection GetConnection(ListenOptions listenOptions)
        {
            var connection = CreateConnection();
            if (listenOptions.PrefetchCount != null)
            {
                if (listenOptions.FromQueue)
                {
                    (connection as Connection).PrefetchPolicy.QueuePrefetch = listenOptions.PrefetchCount.Value;
                }
                else
                {
                    (connection as Connection).PrefetchPolicy.TopicPrefetch = listenOptions.PrefetchCount.Value;
                }
            }
            if (!string.IsNullOrEmpty(listenOptions.ClientId))
            {
                connection.ClientId = listenOptions.ClientId;
            }
            return connection;
        }
        /// <summary>
        /// 获取一个消费者
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listenOptions"></param>
        /// <returns></returns>
        private IMessageConsumer GetMessageConsumer(ISession session, ListenOptions listenOptions)
        {
            IDestination dest = listenOptions.FromQueue ? session.GetQueue(listenOptions.Destination) as IDestination : session.GetTopic(listenOptions.Destination) as IDestination;
            if (listenOptions.Durable && !(dest is ITopic))
            {
                throw new InvalidOperationException($"Durable Subscriber is only for Topic");
            }
            IMessageConsumer consumer = listenOptions.Durable ?
                session.CreateDurableConsumer(dest as ITopic, string.IsNullOrEmpty(listenOptions.SubscriberName) ? listenOptions.ClientId : listenOptions.SubscriberName, listenOptions.Selector, false) :
                session.CreateConsumer(dest, listenOptions.Selector, false);
            return consumer;
        }
        /// <summary>
        /// 开始监听消费消息
        /// </summary>
        /// <param name="listenOptions"></param>
        /// <param name="action"></param>
        /// <param name="listenResult"></param>
        private void ListenInterval(ListenOptions listenOptions, Action<RecieveResult> action, ListenResult listenResult)
        {
            if (listenResult.Stoped) return;

            var connection = GetConnection(listenOptions);
            var registration = listenResult.Token.Register(() =>
            {
                Tracer.Info("Stopping... Listen");
                connection.Dispose();
            });
            if (!connection.IsStarted)
            {
                connection.Start();
            }
            ISession session = connection.CreateSession(listenOptions.AutoAcknowledge ? AcknowledgementMode.AutoAcknowledge : AcknowledgementMode.ClientAcknowledge);
            IMessageConsumer consumer = GetMessageConsumer(session, listenOptions);
            Tracer.Info("Listen Interval:Recieving...");
            if (listenOptions.PrefetchCount <= 0)
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (listenResult.Stoped) break;
                        try
                        {
                            IMessage message = consumer.Receive();
                            OnMessage(message, action);
                        }
                        catch (Exception ex)
                        {
                            while (ex != null)
                            {
                                Tracer.Error($"{ex.GetType().FullName}:{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                                ex = ex.InnerException;
                            }
                        }
                    }
                });
            }
            else
            {
                consumer.Listener += new MessageListener(message =>
                {
                    OnMessage(message, action);
                });
            }

            /// <summary>
            /// 消费消息
            /// </summary>
            /// <param name="message"></param>
            /// <param name="onMessage"></param>
            void OnMessage(IMessage message, Action<RecieveResult> onMessage)
            {
                RecieveResult recieveResult = null;
                if (message is ITextMessage textMessage)
                {
                    recieveResult = new RecieveResult(textMessage);
                }
                if (recieveResult != null)
                {
                    onMessage?.Invoke(recieveResult);
                    if (!recieveResult.IsCommitted && !listenOptions.AutoAcknowledge && listenOptions.RecoverWhenNotAcknowledge)
                    {
                        //通知mq进行重发  最多重发六次
                        session.Recover();
                    }
                }
            }
        }

        #endregion

        #region Listen
        /// <summary>
        /// 阻塞当前线程同步监听并开始消费消息
        /// </summary>
        /// <param name="listenOptions"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        public void Listen(ListenOptions listenOptions, Action<RecieveResult> action = null, CancellationToken cancellationToken = default)
        {
            ListenResult result = new ListenResult();
            cancellationToken.Register(() =>
            {
                result.Stop();
            });
            ListenInterval(listenOptions, action, result);
        }
        /// <summary>
        /// 异步监听并开始消费消息
        /// </summary>
        /// <param name="listenOptions"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ListenResult> ListenAsync(ListenOptions listenOptions, Action<RecieveResult> action = null)
        {
            ListenResult result = new ListenResult();
            new Task(() =>
            {
                ListenInterval(listenOptions, action, result);
            }).Start();
            return await Task.FromResult(result);
        }
        #endregion

        public static ActiveConsumer Create(ActiveBaseOptions activeBaseOptions)
        {
            return new ActiveConsumer(activeBaseOptions.IsCluster, activeBaseOptions.BrokerUris, activeBaseOptions.Query)
            {
                UserName = activeBaseOptions.UserName,
                Password = activeBaseOptions.Password
            };
        }
    }
    public class ListenOptions
    {
        /// <summary>
        /// 是否从队列消费
        /// </summary>
        public bool FromQueue { get; set; }
        /// <summary>
        /// 队列或者Topic
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// 选择器
        /// </summary>
        public string Selector { get; set; }
        /// <summary>
        /// 每次发送消息条数
        /// </summary>
        public int? PrefetchCount { get; set; }
        /// <summary>
        /// 是否自动签收
        /// </summary>
        public bool AutoAcknowledge { get; set; } = true;
        /// <summary>
        /// 重连时间间隔（秒）
        /// </summary>
        public int Interval { get; set; } = 3;
        /// <summary>
        /// 未应答时是否重发消息
        /// </summary>
        public bool RecoverWhenNotAcknowledge { get; set; } = false;
        /// <summary>
        /// 持久化订阅的客户端 Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 持久化订阅名称
        /// </summary>
        public string SubscriberName { get; set; }
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; }
    }
    public class RecieveResult : ActiveMessage
    {
        bool isCommitted = false;
        ITextMessage message;

        internal RecieveResult(ITextMessage message)
        {
            this.message = message;
            this.IsFromQueue = message.NMSDestination.IsQueue;
            this.IsFromTopic = message.NMSDestination.IsTopic;
            this.Destination = IsFromQueue ? (message.NMSDestination as IQueue).QueueName : IsFromTopic ? (message.NMSDestination as ITopic).TopicName : string.Empty;
            this.Message = message.Text;
            this.Redelivered = message.NMSRedelivered;
            this.TimeToLive = message.NMSTimeToLive;
            this.IsPersistent = message.NMSDeliveryMode == MsgDeliveryMode.Persistent;

            foreach (var key in message.Properties.Keys.Cast<string>())
            {
                this.Properties[key] = message.Properties[key];
            }
        }

        /// <summary>
        /// 消息是否来自队列
        /// </summary>
        public bool IsFromQueue { get; private set; }
        /// <summary>
        /// 消息是否来自Topic
        /// </summary>
        public bool IsFromTopic { get; private set; }
        /// <summary>
        /// 消息是否是再次消费
        /// </summary>
        public bool Redelivered { get; private set; }
        /// <summary>
        /// 是否已提交
        /// </summary>
        public bool IsCommitted { get { return isCommitted; } }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            if (!isCommitted)
            {
                isCommitted = true;
                message.Acknowledge();
            }
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
