using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using log4net;
using log4net.Config;

namespace Log4Net.Sample.Common
{
    public sealed class Logger
    {
        /// <summary>
        /// 记录消息Queue
        /// </summary>
        private readonly ConcurrentQueue<LoggerMessage> _que;

        /// <summary>
        /// 信号
        /// </summary>
        private readonly ManualResetEvent _mre;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// 日志
        /// </summary>
        private static Logger _logger = new Logger();

        private Logger()
        {
            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            if (!configFile.Exists)
            {
                throw new Exception("未配置log4net配置文件！");
            }

            // 设置日志配置文件路径
            XmlConfigurator.Configure(configFile);

            _que = new ConcurrentQueue<LoggerMessage>();
            _mre = new ManualResetEvent(false);
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 实现单例
        /// </summary>
        /// <returns></returns>
        public static Logger Instance()
        {
            return _logger;
        }

        /// <summary>
        /// 另一个线程记录日志，只在程序初始化时调用一次
        /// </summary>
        public void Register()
        {
            Thread t = new Thread(new ThreadStart(WriteLog));
            t.IsBackground = false;
            t.Start();
        }

        //从队列中写日志至磁盘
        private void WriteLog()
        {
            while (true)
            {
                // 等待信号通知
                _mre.WaitOne();

                LoggerMessage msg;
                // 判断是否有内容需要如磁盘 从列队中获取内容，并删除列队中的内容
                while (_que.Count > 0 && _que.TryDequeue(out msg))
                {
                    // 判断日志等级，然后写日志
                    switch (msg.Level)
                    {
                        case LoggerLevel.DEBUG:
                            _log.Debug(msg.Message, msg.Exception);
                            break;
                        case LoggerLevel.INFO:
                            _log.Info(msg.Message, msg.Exception);
                            break;
                        case LoggerLevel.ERROR:
                            _log.Error(msg.Message, msg.Exception);
                            break;
                        case LoggerLevel.WARN:
                            _log.Warn(msg.Message, msg.Exception);
                            break;
                        case LoggerLevel.FATAL:
                            _log.Fatal(msg.Message, msg.Exception);
                            break;
                    }
                }

                // 重新设置信号
                _mre.Reset();
                //Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志文本</param>
        /// <param name="level">等级</param>
        /// <param name="ex">Exception</param>
        public void EnqueueMessage(string message, LoggerLevel level, Exception ex = null)
        {
            if ((level == LoggerLevel.DEBUG && _log.IsDebugEnabled)
             || (level == LoggerLevel.ERROR && _log.IsErrorEnabled)
             || (level == LoggerLevel.FATAL && _log.IsFatalEnabled)
             || (level == LoggerLevel.INFO && _log.IsInfoEnabled)
             || (level == LoggerLevel.WARN && _log.IsWarnEnabled))
            {
                _que.Enqueue(new LoggerMessage
                {
                    Message = message,
                    Level = level,
                    Exception = ex
                });

                // 通知线程往磁盘中写日志
                _mre.Set();
            }
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="ex">异常信息</param>
        public static void Debug(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, LoggerLevel.DEBUG, ex);
        }

        /// <summary>
        /// 一般错误
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="ex">异常信息</param>
        public static void Error(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, LoggerLevel.ERROR, ex);
        }

        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="ex">异常信息</param>
        public static void Fatal(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, LoggerLevel.FATAL, ex);
        }

        /// <summary>
        /// 一般信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="ex">异常信息</param>
        public static void Info(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, LoggerLevel.INFO, ex);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="ex">异常信息</param>
        public static void Warn(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, LoggerLevel.WARN, ex);
        }

    }
}
