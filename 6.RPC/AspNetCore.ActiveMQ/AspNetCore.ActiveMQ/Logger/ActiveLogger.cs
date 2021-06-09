using System;
using System.Linq;
using AspNetCore.ActiveMQ.Integration;
using Microsoft.Extensions.Logging;

namespace AspNetCore.ActiveMQ.Logger
{
    public class ActiveLogger : ILogger, IDisposable
    {
        static string ip = "";

        string category;
        ActiveLoggerOptions loggerOptions;
        ActiveProducer producer;

        public ActiveLogger(string category, ActiveLoggerOptions options, ActiveProducer producer)
        {
            this.category = category ?? "";
            this.loggerOptions = options;
            this.producer = producer;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
        /// <summary>
        /// 是否记录日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            //只记录日志等级大于指定最小等级且属于Active分类的日志
            return logLevel >= loggerOptions.MinLevel && category.ToLower().Contains((loggerOptions.Category ?? "").ToLower());
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (string.IsNullOrEmpty(ip))
            {
                lock (ip)
                {
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = GetLocalIp();
                    }
                }
            }

            if (IsEnabled(logLevel))
            {
                try
                {
                    var message = new ActiveLoggerMessage<TState>
                    {
                        Category = category,
                        Error = ActiveLoggerError.Parse(exception),
                        LogLevel = logLevel,
                        State = state,
                        Message = formatter?.Invoke(state, exception),
                        ApplicationName = loggerOptions.ApplicationName,
                        Time = DateTime.Now,
                        IpAddress = ip,
                    };
                    var json = loggerOptions.Serialize(message);
                    //发送消息
                    if (loggerOptions.UseQueue)
                    {
                        producer.Send(new ActiveMessage()
                        {
                            Destination = loggerOptions.Destination,
                            IsPersistent = loggerOptions.IsPersistent,
                            Message = json,
                            Properties = loggerOptions.Properties,
                            TimeToLive = loggerOptions.TimeToLive
                        });
                    }
                    else
                    {
                        producer.Publish(new ActiveMessage()
                        {
                            Destination = loggerOptions.Destination,
                            IsPersistent = loggerOptions.IsPersistent,
                            Message = json,
                            Properties = loggerOptions.Properties,
                            TimeToLive = loggerOptions.TimeToLive
                        });
                    }
                }
                catch (Exception ex)
                {
                    do
                    {
                        Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    } while ((ex = ex.InnerException) != null);
                };
            }
        }
        /// <summary>
        /// 获取本地Ip地址
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIp()
        {
            var addressList = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
            var ips = addressList.Where(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(address => address.ToString()).ToArray();
            if (ips.Length == 1)
            {
                return ips.First();
            }
            return ips.Where(address => !address.EndsWith(".1")).FirstOrDefault() ?? ips.FirstOrDefault();
        }
    }
}
