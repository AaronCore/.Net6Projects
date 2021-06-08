using AspNetCore.RabbitMQ.Integration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCore.RabbitMQ.Logger
{
    public class RabbitLogger : ILogger, IDisposable
    {
        static string ip = "";

        string category;
        RabbitLoggerOptions loggerOptions;
        RabbitLoggerProvider rabbitLoggerProvider;

        public RabbitLogger(string category, RabbitLoggerOptions options, RabbitLoggerProvider rabbitLoggerProvider)
        {
            this.category = category ?? "";
            this.loggerOptions = options;
            this.rabbitLoggerProvider = rabbitLoggerProvider;
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
            //只记录日志等级大于指定最小等级且属于Rabbit分类的日志
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
                var message = new RabbitLoggerMessage<TState>
                {
                    Error = RabbitLoggerError.Parse(exception),
                    LogLevel = logLevel,
                    State = state,
                    Message = formatter?.Invoke(state, exception),
                    Category = category,
                    ApplicationName = loggerOptions.ApplicationName,
                    Time = DateTime.Now,
                    IpAddress = ip,
                };

                try
                {
                    var producer = rabbitLoggerProvider.RentProducer();
                    //发送消息
                    if (loggerOptions.Type == RabbitExchangeType.None)
                    {
                        producer.Publish(loggerOptions.Queue, JsonConvert.SerializeObject(message), new QueueOptions()
                        {
                            Arguments = loggerOptions.Arguments,
                            AutoDelete = loggerOptions.AutoDelete,
                            Durable = loggerOptions.Durable
                        });
                    }
                    else
                    {
                        producer.Publish(loggerOptions.Exchange, category, JsonConvert.SerializeObject(message), new ExchangeQueueOptions()
                        {
                            Type = loggerOptions.Type,
                            RouteQueues = loggerOptions.RouteQueues,
                            Arguments = loggerOptions.Arguments,
                            AutoDelete = loggerOptions.AutoDelete,
                            Durable = loggerOptions.Durable
                        });
                    }
                    rabbitLoggerProvider.ReturnProducer(producer);
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
