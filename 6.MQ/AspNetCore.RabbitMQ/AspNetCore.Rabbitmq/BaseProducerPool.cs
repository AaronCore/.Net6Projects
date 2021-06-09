using AspNetCore.RabbitMQ.Integration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ
{
    public abstract class BaseProducerPool : IDisposable
    {
        ConcurrentQueue<RabbitProducer> rabbitProducers = new ConcurrentQueue<RabbitProducer>();
        RabbitOptions rabbitOptions;

        protected BaseProducerPool(RabbitOptions rabbitOptions)
        {
            this.rabbitOptions = rabbitOptions;
            this.rabbitProducers = new ConcurrentQueue<RabbitProducer>();
        }

        public bool Disposed { get; private set; } = false;
        /// <summary>
        /// 保留发布者数
        /// </summary>
        protected virtual int InitializeCount { get; } = 5;

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                while (rabbitProducers.Count > 0)
                {
                    RabbitProducer rabbitProducer;
                    rabbitProducers.TryDequeue(out rabbitProducer);
                    rabbitProducer?.Dispose();
                }
            }
        }
        /// <summary>
        /// 租赁一个发布者
        /// </summary>
        /// <returns></returns>
        public RabbitProducer RentProducer()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitClientProducer));
            }

            RabbitProducer rabbitProducer;
            lock (rabbitProducers)
            {
                if (!rabbitProducers.TryDequeue(out rabbitProducer) || rabbitProducer == null || !rabbitProducer.IsOpen)
                {
                    rabbitProducer = RabbitProducer.Create(rabbitOptions);
                }
            }
            return rabbitProducer;
        }
        /// <summary>
        /// 返回保存发布者
        /// </summary>
        /// <param name="rabbitProducer"></param>
        public void ReturnProducer(RabbitProducer rabbitProducer)
        {
            if (Disposed) return;

            lock (rabbitProducers)
            {
                if (rabbitProducers.Count < InitializeCount && rabbitProducer != null && rabbitProducer.IsOpen)
                {
                    rabbitProducers.Enqueue(rabbitProducer);
                }
                else
                {
                    rabbitProducer?.Dispose();
                }
            }
        }
    }
}
