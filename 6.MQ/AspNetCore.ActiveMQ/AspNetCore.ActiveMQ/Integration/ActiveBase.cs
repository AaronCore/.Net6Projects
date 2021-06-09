using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ActiveMQ.Integration
{
    public abstract class ActiveBase : IDisposable
    {
#if DEBUG
        public static ActiveTrace Trace { get; set; } = new ActiveTrace(TraceLevel.Warn, (level, message) => Console.WriteLine($"{level}:{message}"));
#else
        public static ActiveTrace Trace { get; set; } = null;
#endif

        static ActiveBase()
        {
            Tracer.Trace = Trace;
        }

        const int DefaultPort = 61616;
        const string DefaultHost = "localhost";

        IConnectionFactory connectionFactory;


        bool isDisposed = false;
        string brokerUri;

        protected ActiveBase(bool isCluster, string[] hostAndPorts, string query)
        {
            if (hostAndPorts == null || hostAndPorts.Length == 0)
            {
                throw new ArgumentException("invalid hostAndPorts！", nameof(hostAndPorts));
            }
            if (!isCluster && hostAndPorts?.Length != 1)
            {
                throw new ArgumentException("None Cluster mode only support one host", nameof(hostAndPorts));
            }
            if (isCluster)
            {
                brokerUri = MakeClusterBrokerUri(hostAndPorts, query);
            }
            else
            {
                brokerUri = MakeBrokerUri(hostAndPorts.First(), query);
            }
            connectionFactory = CreateConnectionFactory(brokerUri);
        }

        public string UserName { get; set; }
        public string Password { get; set; }

        private IConnectionFactory CreateConnectionFactory(string brokerUri)
        {
            NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);
            return factory;
        }
        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <returns></returns>
        protected IConnection CreateConnection()
        {
            CheckDisposed();
            IConnection connection = connectionFactory.CreateConnection(UserName, Password);
            return connection;
        }
        protected void CheckDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
        public virtual void Dispose()
        {
            isDisposed = true;
        }
        public override string ToString()
        {
            return $"{this.GetType().FullName}:{brokerUri}";
        }

        private static string MakeClusterBrokerUri(string[] hostAndPorts, string query)
        {
            List<string> list = new List<string>();
            foreach (var hostAndPort in hostAndPorts)
            {
                var splits = hostAndPort.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length == 1)
                {
                    list.Add($"tcp://{splits[0]}:{DefaultPort}");
                }
                else if (splits.Length == 2)
                {
                    list.Add($"tcp://{splits[0]}:{splits[1]}");
                }
                else
                {
                    throw new ArgumentException($"invalid {nameof(hostAndPort)}:{hostAndPort}");
                }
            }
            string uri = string.Join(",", list);
            query = query?.Trim()?.TrimStart('?');
            return string.IsNullOrEmpty(query) ? $"activemq:failover:({uri})" : $"activemq:failover:({uri})?{query}";
        }
        private static string MakeBrokerUri(string hostAndPort, string query)
        {
            var splits = hostAndPort.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            var uri = "";
            if (splits.Length == 1)
            {
                uri = $"tcp://{splits[0]}:{DefaultPort}";
            }
            else if (splits.Length == 2)
            {
                uri = $"tcp://{splits[0]}:{splits[1]}";
            }
            else
            {
                throw new ArgumentException($"invalid {nameof(hostAndPort)}:{hostAndPort}");
            }
            query = query?.Trim()?.TrimStart('?');
            return string.IsNullOrEmpty(query) ? $"activemq:{uri}" : $"activemq:{uri}?{query}";
        }

    }
}
