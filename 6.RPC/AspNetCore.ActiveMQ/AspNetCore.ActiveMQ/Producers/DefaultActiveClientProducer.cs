using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ActiveMQ.Integration;

namespace AspNetCore.ActiveMQ.Producers
{
    public class DefaultActiveClientProducer : IActiveClientProducer
    {
        ActiveProducerOptions activeProducerOptions;
        ActiveProducer producer;

        public DefaultActiveClientProducer(ActiveProducerOptions activeProducerOptions)
        {
            this.activeProducerOptions = activeProducerOptions;
        }

        public bool Disposed { get; private set; }

        private ActiveProducer GetProducer()
        {
            if (producer == null)
            {
                lock (this)
                {
                    if (producer == null)
                    {
                        producer = ActiveProducer.Create(activeProducerOptions);
                        producer.InitializeCount = activeProducerOptions.InitializeCount;
                    }
                }
            }
            return producer;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                producer?.Dispose();
            }
        }

        /// <summary>
        /// 发送消息到Topic
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <returns></returns>
        public async Task PublishAsync(params ActiveMessage[] activeMessages)
        {
            if (activeMessages != null && activeMessages.Length > 0)
            {
                var producer = GetProducer();

                await producer.PublishAsync(activeMessages.Select(f => new ActiveMessage()
                {
                    Destination = string.IsNullOrEmpty(f.Destination) ? activeProducerOptions.Destination : f.Destination,
                    IsPersistent = f.IsPersistent == null ? activeProducerOptions.IsPersistent : f.IsPersistent,
                    Message = f.Message,
                    Properties = f.Properties == null ? activeProducerOptions.Properties : f.Properties,
                    TimeToLive = f.TimeToLive == null ? activeProducerOptions.TimeToLive : f.TimeToLive
                }).ToArray(), new ActiveMessageOptions()
                {
                    Transactional = activeProducerOptions.Transactional
                });
            }
        }
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="activeMessages"></param>
        /// <returns></returns>
        public async Task SendAsync(params ActiveMessage[] activeMessages)
        {
            if (activeMessages != null && activeMessages.Length > 0)
            {
                var producer = GetProducer();

                await producer.SendAsync(activeMessages.Select(f => new ActiveMessage()
                {
                    Destination = string.IsNullOrEmpty(f.Destination) ? activeProducerOptions.Destination : f.Destination,
                    IsPersistent = f.IsPersistent == null ? activeProducerOptions.IsPersistent : f.IsPersistent,
                    Message = f.Message,
                    Properties = f.Properties == null ? activeProducerOptions.Properties : f.Properties,
                    TimeToLive = f.TimeToLive == null ? activeProducerOptions.TimeToLive : f.TimeToLive
                }).ToArray(), new ActiveMessageOptions()
                {
                    Transactional = activeProducerOptions.Transactional
                });
            }
        }
    }
}
