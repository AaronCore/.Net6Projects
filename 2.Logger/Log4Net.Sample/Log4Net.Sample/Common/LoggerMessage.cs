using System;

namespace Log4Net.Sample.Common
{
    /// <summary>
    /// 日志内容
    /// </summary>
    public class LoggerMessage
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public LoggerLevel Level { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }
    }
}
