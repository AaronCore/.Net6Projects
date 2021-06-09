using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ActiveMQ.Integration
{
    public class ActiveProducer : ActiveBase
    {
        ConcurrentQueue<IConnection> connectionQueue = new ConcurrentQueue<IConnection>();

        public ActiveProducer(bool isCluster, string[] hostAndPorts, string query = null) :
            base(isCluster, hostAndPorts, query)
        {

        }

        /// <summary>
        /// 保留连接数
        /// </summary>
        public int InitializeCount { get; set; } = 5;

        #region Private
        /// <summary>
        /// 获取一个连接
        /// </summary>
        /// <returns></returns>
        private IConnection RentConnection()
        {
            CheckDisposed();

            while (connectionQueue.TryDequeue(out IConnection connection))
            {
                if (connection != null)
                {
                    var con = connection as Connection;
                    if (con.ITransport.IsConnected)
                    {
                        return con;
                    }
                }
            }

            return CreateConnection();
        }
        /// <summary>
        /// 返还一个连接
        /// </summary>
        /// <param name="connection"></param>
        private void ReturnConnection(IConnection connection)
        {
            CheckDisposed();

            lock (connectionQueue)
            {
                if (connectionQueue.Count < InitializeCount && connection != null && (connection as Connection).ITransport.IsConnected)
                {
                    connectionQueue.Enqueue(connection);
                }
                else
                {
                    connection?.Dispose();
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="activeMessageOptions"></param>
        /// <param name="activeMessages"></param>
        /// <param name="func"></param>
        /// <param name="maxTime"></param>
        private void SendMessage(ActiveMessageOptions activeMessageOptions, ActiveMessage[] activeMessages, Func<ISession, string, IDestination> func)
        {
            var connection = RentConnection();
            if (!connection.IsStarted)
            {
                connection.Start();
            }

            activeMessageOptions = activeMessageOptions ?? new ActiveMessageOptions();
            using (ISession session = connection.CreateSession(activeMessageOptions.Transactional ? AcknowledgementMode.Transactional : AcknowledgementMode.AutoAcknowledge))
            {
                ReturnConnection(connection);//创建会话后不用等会话结束

                try
                {
                    IMessageProducer producer = session.CreateProducer();
                    Dictionary<string, IDestination> dict = new Dictionary<string, IDestination>();
                    foreach (var activeMessage in activeMessages)
                    {
                        if (!dict.ContainsKey(activeMessage.Destination))
                        {
                            var _dest = func.Invoke(session, activeMessage.Destination);
                            dict[activeMessage.Destination] = _dest;
                        }
                        var dest = dict[activeMessage.Destination];

                        var message = producer.CreateTextMessage(activeMessage.Message);
                        if (activeMessage.Properties != null)
                        {
                            foreach (var item in activeMessage.Properties)
                            {
                                message.Properties[item.Key] = item.Value;
                            }
                        }

                        producer.DeliveryMode = activeMessage.IsPersistent == null || activeMessage.IsPersistent.Value ? MsgDeliveryMode.Persistent : MsgDeliveryMode.NonPersistent;
                        producer.Priority = MsgPriority.Normal;
                        if (activeMessage.TimeToLive != null)
                        {
                            producer.Send(dest, message, producer.DeliveryMode, producer.Priority, activeMessage.TimeToLive.Value);
                        }
                        else
                        {
                            producer.Send(dest, message);
                        }
                    }
                    if (activeMessageOptions.Transactional)
                    {
                        session.Commit();
                    }
                }
                catch
                {
                    if (activeMessageOptions.Transactional)
                    {
                        session.Rollback();
                    }

                    throw;
                }
            }
        }

        #endregion

        #region Queue
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        /// <param name="activeMessageOptions"></param>
        public void Send(string queue, string message, ActiveMessageOptions activeMessageOptions = null)
        {
            Send(queue, new string[] { message }, activeMessageOptions);
        }
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="messages"></param>
        /// <param name="activeMessageOptions"></param>
        public void Send(string queue, string[] messages, ActiveMessageOptions activeMessageOptions = null)
        {
            Send(messages.Select(f => new ActiveMessage()
            {
                Destination = queue,
                Message = f
            }).ToArray(), activeMessageOptions);
        }
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="activeMessage"></param>
        /// <param name="activeMessageOptions"></param>
        public void Send(ActiveMessage activeMessage, ActiveMessageOptions activeMessageOptions = null)
        {
            Send(new ActiveMessage[] { activeMessage }, activeMessageOptions);
        }
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <param name="activeMessageOptions"></param>
        public void Send(ActiveMessage[] activeMessages, ActiveMessageOptions activeMessageOptions = null)
        {
            SendMessage(activeMessageOptions, activeMessages, (session, destination) => session.GetQueue(destination));
        }
        /// <summary>
        /// 异步发送消息到队列
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task SendAsync(string queue, string message, ActiveMessageOptions activeMessageOptions = null)
        {
            await SendAsync(queue, new string[] { message }, activeMessageOptions);
        }
        /// <summary>
        /// 异步发送消息到队列
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="messages"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task SendAsync(string queue, string[] messages, ActiveMessageOptions activeMessageOptions = null)
        {
            await SendAsync(messages.Select(f => new ActiveMessage()
            {
                Destination = queue,
                Message = f
            }).ToArray(), activeMessageOptions);
        }
        /// <summary>
        /// 异步发送消息到队列
        /// </summary>
        /// <param name="activeMessage"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task SendAsync(ActiveMessage activeMessage, ActiveMessageOptions activeMessageOptions = null)
        {
            await SendAsync(new ActiveMessage[] { activeMessage }, activeMessageOptions);
        }
        /// <summary>
        /// 异步发送消息到队列
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task SendAsync(ActiveMessage[] activeMessages, ActiveMessageOptions activeMessageOptions = null)
        {
            await Task.Run(() =>
            {
                Send(activeMessages, activeMessageOptions);
            });
        }
        #endregion

        #region Topic
        /// <summary>
        /// 发布消息到Topic
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="activeMessageOptions"></param>
        public void Publish(string topic, string message, ActiveMessageOptions activeMessageOptions = null)
        {
            Publish(topic, new string[] { message }, activeMessageOptions);
        }
        /// <summary>
        /// 发布消息到Topic
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="messages"></param>
        /// <param name="activeMessageOptions"></param>
        public void Publish(string topic, string[] messages, ActiveMessageOptions activeMessageOptions = null)
        {
            Publish(messages.Select(f => new ActiveMessage()
            {
                Destination = topic,
                Message = f
            }).ToArray(), activeMessageOptions);
        }
        /// <summary>
        /// 发布消息到Topic
        /// </summary>
        /// <param name="activeMessage"></param>
        /// <param name="activeMessageOptions"></param>
        public void Publish(ActiveMessage activeMessage, ActiveMessageOptions activeMessageOptions = null)
        {
            Publish(new ActiveMessage[] { activeMessage }, activeMessageOptions);
        }
        /// <summary>
        /// 发布消息到Topic
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <param name="activeMessageOptions"></param>
        public void Publish(ActiveMessage[] activeMessages, ActiveMessageOptions activeMessageOptions = null)
        {
            SendMessage(activeMessageOptions, activeMessages, (session, destination) => session.GetTopic(destination));
        }
        /// <summary>
        /// 异步发布消息到Topic
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task PublishAsync(string topic, string message, ActiveMessageOptions activeMessageOptions = null)
        {
            await PublishAsync(topic, new string[] { message }, activeMessageOptions);
        }
        /// <summary>
        /// 异步发布消息到Topic
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="messages"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task PublishAsync(string topic, string[] messages, ActiveMessageOptions activeMessageOptions = null)
        {
            await PublishAsync(messages.Select(f => new ActiveMessage()
            {
                Destination = topic,
                Message = f
            }).ToArray(), activeMessageOptions);
        }
        /// <summary>
        /// 异步发布消息到Topic
        /// </summary>
        /// <param name="activeMessage"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task PublishAsync(ActiveMessage activeMessage, ActiveMessageOptions activeMessageOptions = null)
        {
            await PublishAsync(new ActiveMessage[] { activeMessage }, activeMessageOptions);
        }
        /// <summary>
        /// 异步发布消息到Topic
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <param name="activeMessageOptions"></param>
        /// <returns></returns>
        public async Task PublishAsync(ActiveMessage[] activeMessages, ActiveMessageOptions activeMessageOptions = null)
        {
            await Task.Run(() =>
            {
                Publish(activeMessages, activeMessageOptions);
            });
        }
        #endregion

        public static ActiveProducer Create(ActiveBaseOptions activeBaseOptions)
        {
            return new ActiveProducer(activeBaseOptions.IsCluster, activeBaseOptions.BrokerUris, activeBaseOptions.Query)
            {
                UserName = activeBaseOptions.UserName,
                Password = activeBaseOptions.Password
            };
        }

        public override void Dispose()
        {
            base.Dispose();

            while (connectionQueue.Count > 0)
            {
                connectionQueue.TryDequeue(out IConnection connection);
                connection?.Dispose();
            }
        }
    }

}
