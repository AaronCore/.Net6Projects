using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AspNetCore.ActiveMQ.Logger
{
    public class ActiveLoggerMessage<TState>
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
        public ActiveLoggerError Error { get; set; }
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
    public class ActiveLoggerError
    {
        public ActiveLoggerError()
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

        public ActiveLoggerError InnerError { get; set; }
        public ActiveLoggerError[] InnerErrors { get; set; }

        public static ActiveLoggerError Parse(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            var error = new ActiveLoggerError()
            {
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerError = Parse(exception.InnerException)
            };

            List<ActiveLoggerError> list = new List<ActiveLoggerError>();
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
