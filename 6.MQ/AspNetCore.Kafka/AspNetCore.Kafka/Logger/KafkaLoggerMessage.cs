using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Kafka.Logger
{
    public class KafkaLoggerMessage<TState>
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// 状态值
        /// </summary>
        public TState State { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public KafkaLoggerError Error { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;
        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress { get; set; }
    }
    public class KafkaLoggerError
    {
        public KafkaLoggerError()
        {

        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 堆栈信息
        /// </summary>
        public string StackTrace { get; set; }

        public KafkaLoggerError InnerError { get; set; }
        public KafkaLoggerError[] InnerErrors { get; set; }

        public static KafkaLoggerError Parse(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            var error = new KafkaLoggerError()
            {
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerError = Parse(exception.InnerException)
            };

            List<KafkaLoggerError> list = new List<KafkaLoggerError>();
            if (exception is AggregateException aggregateException)
            {
                foreach (var ex in aggregateException.InnerExceptions)
                {
                    list.Add(Parse(ex));
                }
            }
            error.InnerErrors = list.ToArray();

            return error;
        }
    }
}
