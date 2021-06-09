using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Kafka.Integration
{
    public class KafkaProducer : KafkaBase, IDisposable
    {
        /// <summary>
        /// 负责生成producer
        /// </summary>
        ProducerBuilder<string, object> builder;
        ConcurrentQueue<IProducer<string, object>> producers;
        bool disposed = false;

        /// <summary>
        /// Flush超时时间(ms)
        /// </summary>
        public int FlushTimeOut { get; set; } = 10000;
        /// <summary>
        /// 保留发布者数
        /// </summary>
        public int InitializeCount { get; set; } = 5;
        /// <summary>
        /// 默认的消息键值
        /// </summary>
        public string DefaultKey { get; set; }
        /// <summary>
        /// 默认的主题
        /// </summary>
        public string DefaultTopic { get; set; }
        /// <summary>
        /// 异常事件
        /// </summary>
        public event Action<object, Exception> ErrorHandler;
        /// <summary>
        /// 统计事件
        /// </summary>
        public event Action<object, string> StatisticsHandler;
        /// <summary>
        /// 日志事件
        /// </summary>
        public event Action<object, KafkaLogMessage> LogHandler;

        public KafkaProducer(params string[] bootstrapServers)
        {
            if (bootstrapServers == null || bootstrapServers.Length == 0)
            {
                throw new Exception("at least one server must be assigned");
            }

            this.BootstrapServers = string.Join(",", bootstrapServers);
            producers = new ConcurrentQueue<IProducer<string, object>>();
        }

        #region Private
        /// <summary>
        /// producer构造器
        /// </summary>
        /// <returns></returns>
        private ProducerBuilder<string, object> CreateProducerBuilder()
        {
            if (builder == null)
            {
                lock (this)
                {
                    if (builder == null)
                    {
                        ProducerConfig config = new ProducerConfig();
                        config.BootstrapServers = BootstrapServers;
                        if (!string.IsNullOrEmpty(SaslUsername))
                        {
                            config.SaslUsername = SaslUsername;
                            config.SaslPassword = SaslPassword;
                            config.SaslMechanism = SaslMechanism.Plain;
                            config.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                        }

                        //List<KeyValuePair<string, string>> config = new List<KeyValuePair<string, string>>();
                        //config.Add(new KeyValuePair<string, string>("bootstrap.servers", BootstrapServers));
                        //if (!string.IsNullOrEmpty(SaslUsername))
                        //{
                        //    config.Add(new KeyValuePair<string, string>("security.protocol", "SASL_PLAINTEXT"));
                        //    config.Add(new KeyValuePair<string, string>("sasl.mechanism", "PLAIN"));
                        //    config.Add(new KeyValuePair<string, string>("sasl.username", SaslUsername));
                        //    config.Add(new KeyValuePair<string, string>("sasl.password", SaslPassword));
                        //}

                        builder = new ProducerBuilder<string, object>(config);
                        Action<Delegate, object> tryCatchWrap = (@delegate, arg) =>
                        {
                            try
                            {
                                @delegate?.DynamicInvoke(arg);
                            }
                            catch { }
                        };
                        builder.SetErrorHandler((p, e) => tryCatchWrap(ErrorHandler, new Exception(e.Reason)));
                        builder.SetStatisticsHandler((p, e) => tryCatchWrap(StatisticsHandler, e));
                        builder.SetLogHandler((p, e) => tryCatchWrap(LogHandler, new KafkaLogMessage(e)));
                        builder.SetValueSerializer(new KafkaConverter());
                    }
                }
            }

            return builder;
        }
        /// <summary>
        /// 租赁一个发布者
        /// </summary>
        /// <returns></returns>
        private IProducer<string, object> RentProducer()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaProducer));
            }

            IProducer<string, object> producer;
            lock (producers)
            {
                if (!producers.TryDequeue(out producer) || producer == null)
                {
                    CreateProducerBuilder();
                    producer = builder.Build();
                }
            }
            return producer;
        }
        /// <summary>
        /// 返回保存发布者
        /// </summary>
        /// <param name="producer"></param>
        private void ReturnProducer(IProducer<string, object> producer)
        {
            if (disposed) return;

            lock (producers)
            {
                if (producers.Count < InitializeCount && producer != null)
                {
                    producers.Enqueue(producer);
                }
                else
                {
                    producer?.Dispose();
                }
            }
        }
        #endregion

        #region Publish
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public void Publish(object message, Action<DeliveryResult> callback = null)
        {
            Publish(DefaultTopic, null, DefaultKey, message, callback);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public void Publish(string topic, object message, Action<DeliveryResult> callback = null)
        {
            Publish(topic, null, DefaultKey, message, callback);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public void Publish(string topic, int? partition, object message, Action<DeliveryResult> callback = null)
        {
            Publish(topic, partition, DefaultKey, message, callback);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public void Publish(string topic, int? partition, string key, object message, Action<DeliveryResult> callback = null)
        {
            Publish(new KafkaMessage() { Key = key, Message = message, Partition = partition, Topic = topic }, callback);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="kafkaMessage"></param>
        /// <param name="callback"></param>
        public void Publish(KafkaMessage kafkaMessage, Action<DeliveryResult> callback = null)
        {
            var topic = string.IsNullOrEmpty(kafkaMessage.Topic) ? DefaultTopic : kafkaMessage.Topic;
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("topic can not be empty", nameof(kafkaMessage.Topic));
            }
            var key = string.IsNullOrEmpty(kafkaMessage.Key) ? DefaultKey : kafkaMessage.Key;
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key can not be empty", nameof(kafkaMessage.Key));
            }

            var producer = RentProducer();
            if (kafkaMessage.Partition == null)
            {
                producer.Produce(topic, new Message<string, object>() { Key = key, Value = kafkaMessage.Message }, dr => callback?.Invoke(new DeliveryResult(dr)));
            }
            else
            {
                var topicPartition = new TopicPartition(topic, new Partition(kafkaMessage.Partition.Value));
                producer.Produce(topicPartition, new Message<string, object>() { Key = key, Value = kafkaMessage.Message }, dr => callback?.Invoke(new DeliveryResult(dr)));
            }

            producer.Flush(TimeSpan.FromMilliseconds(FlushTimeOut));

            ReturnProducer(producer);
        }
        #endregion

        #region PublishAsync
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="message"></param>
        public async Task<DeliveryResult> PublishAsync(object message)
        {
            return await PublishAsync(DefaultTopic, null, DefaultKey, message);
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        public async Task<DeliveryResult> PublishAsync(string topic, object message)
        {
            return await PublishAsync(topic, null, DefaultKey, message);
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        /// <param name="message"></param>
        public async Task<DeliveryResult> PublishAsync(string topic, int? partition, object message)
        {
            return await PublishAsync(topic, partition, DefaultKey, message);
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<DeliveryResult> PublishAsync(string topic, int? partition, string key, object message)
        {
            return await PublishAsync(new KafkaMessage() { Key = key, Message = message, Partition = partition, Topic = topic });
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="kafkaMessage"></param>
        /// <returns></returns>
        public async Task<DeliveryResult> PublishAsync(KafkaMessage kafkaMessage)
        {
            var topic = string.IsNullOrEmpty(kafkaMessage.Topic) ? DefaultTopic : kafkaMessage.Topic;
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("topic can not be empty", nameof(kafkaMessage.Topic));
            }
            var key = string.IsNullOrEmpty(kafkaMessage.Key) ? DefaultKey : kafkaMessage.Key;
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key can not be empty", nameof(kafkaMessage.Key));
            }

            var producer = RentProducer();
            DeliveryResult<string, object> deliveryResult;
            if (kafkaMessage.Partition == null)
            {
                deliveryResult = await producer.ProduceAsync(topic, new Message<string, object>() { Key = key, Value = kafkaMessage.Message });
            }
            else
            {
                var topicPartition = new TopicPartition(topic, new Partition(kafkaMessage.Partition.Value));
                deliveryResult = await producer.ProduceAsync(topicPartition, new Message<string, object>() { Key = key, Value = kafkaMessage.Message });
            }

            producer.Flush(new TimeSpan(0, 0, 0, 0, FlushTimeOut));

            ReturnProducer(producer);

            return new DeliveryResult(deliveryResult);
        }

        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            while (producers.Count > 0)
            {
                IProducer<string, object> producer;
                producers.TryDequeue(out producer);
                producer?.Dispose();
            }
            GC.Collect();
        }

        public static KafkaProducer Create(KafkaBaseOptions kafkaBaseOptions)
        {
            return new KafkaProducer(kafkaBaseOptions.BootstrapServers)
            {
                SaslUsername = kafkaBaseOptions.SaslUsername,
                SaslPassword = kafkaBaseOptions.SaslPassword
            };
        }
        public override string ToString()
        {
            return BootstrapServers;
        }
    }

    public class DeliveryResult
    {
        internal DeliveryResult(DeliveryResult<string, object> deliveryResult)
        {
            this.Topic = deliveryResult.Topic;
            this.Partition = deliveryResult.Partition.Value;
            this.Offset = deliveryResult.Offset.Value;
            switch (deliveryResult.Status)
            {
                case PersistenceStatus.NotPersisted: this.Status = DeliveryResultStatus.NotPersisted; break;
                case PersistenceStatus.Persisted: this.Status = DeliveryResultStatus.Persisted; break;
                case PersistenceStatus.PossiblyPersisted: this.Status = DeliveryResultStatus.PossiblyPersisted; break;
            }
            this.Key = deliveryResult.Key;
            this.Message = deliveryResult.Value;

            if (deliveryResult is DeliveryReport<string, object>)
            {
                var dr = deliveryResult as DeliveryReport<string, object>;
                this.IsError = dr.Error.IsError;
                this.Reason = dr.Error.Reason;
            }
        }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool IsError { get; private set; }
        /// <summary>
        /// 异常原因
        /// </summary>
        public string Reason { get; private set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; private set; }
        /// <summary>
        /// 分区
        /// </summary>
        public int Partition { get; private set; }
        /// <summary>
        /// 偏移
        /// </summary>
        public long Offset { get; private set; }
        /// <summary>
        /// 状态
        /// </summary>
        public DeliveryResultStatus Status { get; private set; }
        /// <summary>
        /// 消息键值
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 消息
        /// </summary>
        public object Message { get; private set; }
    }
    public enum DeliveryResultStatus
    {
        /// <summary>
        /// 消息提交失败
        /// </summary>
        NotPersisted = 0,
        /// <summary>
        /// 消息已提交，是否成功未知
        /// </summary>
        PossiblyPersisted = 1,
        /// <summary>
        /// 消息提交成功
        /// </summary>
        Persisted = 2
    }
    public class KafkaLogMessage
    {
        internal KafkaLogMessage(LogMessage logMessage)
        {
            this.Name = logMessage.Name;
            this.Facility = logMessage.Facility;
            this.Message = logMessage.Message;

            switch (logMessage.Level)
            {
                case SyslogLevel.Emergency: this.Level = LogLevel.Emergency; break;
                case SyslogLevel.Alert: this.Level = LogLevel.Alert; break;
                case SyslogLevel.Critical: this.Level = LogLevel.Critical; break;
                case SyslogLevel.Error: this.Level = LogLevel.Error; break;
                case SyslogLevel.Warning: this.Level = LogLevel.Warning; break;
                case SyslogLevel.Notice: this.Level = LogLevel.Notice; break;
                case SyslogLevel.Info: this.Level = LogLevel.Info; break;
                case SyslogLevel.Debug: this.Level = LogLevel.Debug; break;
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 级别
        /// </summary>
        public LogLevel Level { get; private set; }
        /// <summary>
        /// 装置
        /// </summary>
        public string Facility { get; private set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; private set; }

        public enum LogLevel
        {
            Emergency = 0,
            Alert = 1,
            Critical = 2,
            Error = 3,
            Warning = 4,
            Notice = 5,
            Info = 6,
            Debug = 7
        }
    }
}
