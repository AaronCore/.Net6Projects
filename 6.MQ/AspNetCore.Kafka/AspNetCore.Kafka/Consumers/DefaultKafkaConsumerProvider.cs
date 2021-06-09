using AspNetCore.Kafka.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Kafka.Consumers
{
    public class DefaultKafkaConsumerProvider : IKafkaConsumerProvider
    {
        KafkaConsumerOptions kafkaConsumerOptions;
        ListenResult listenResult;
        Action<RecieveResult> onMessageRecieved;
        bool disposed = false;

        public DefaultKafkaConsumerProvider(KafkaConsumerOptions kafkaConsumerOptions, Action<RecieveResult> onMessageRecieved)
        {
            this.kafkaConsumerOptions = kafkaConsumerOptions;
            this.onMessageRecieved = onMessageRecieved;

            Consumer = KafkaConsumer.Create(kafkaConsumerOptions.GroupId, kafkaConsumerOptions);
            Consumer.EnableAutoCommit = kafkaConsumerOptions.EnableAutoCommit;
        }

        public KafkaConsumer Consumer { get; }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                listenResult?.Stop();
                Consumer?.Dispose();
            }
        }


        public async Task ListenAsync()
        {
            if (disposed) throw new ObjectDisposedException(nameof(DefaultKafkaConsumerProvider));

            if (listenResult == null)
            {
                listenResult = await Consumer.ListenAsync(kafkaConsumerOptions.Subscribers, recieveResult =>
                {
                    onMessageRecieved.Invoke(recieveResult);
                });
            }
        }
        public override string ToString()
        {
            return $"Consumer-{Consumer}    Subscribers:{string.Join(",", kafkaConsumerOptions.Subscribers.Select(f => f.Partition == null ? f.Topic : $"{f.Topic}:{f.Partition}"))}";
        }
    }
}
