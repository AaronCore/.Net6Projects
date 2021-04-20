using System.ComponentModel;

namespace Log4Net.Sample.Common
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LoggerLevel
    {
        [Description("调试信息")]
        DEBUG,
        [Description("一般信息")]
        INFO,
        [Description("一般错误")]
        ERROR,
        [Description("警告")]
        WARN,
        [Description("致命错误")]
        FATAL
    }
}
