using System;
using System.IO;

namespace NLog.Sample
{
    public class NLogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly object LogWriteLockObj = new object();

        /// <summary>
        /// 非常详细的日志
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message)
        {
            Logger.Trace(message);
        }

        /// <summary>
        /// 非常详细的日志
        /// </summary>
        /// <param name="exception"></param>
        public static void Trace(Exception exception)
        {
            Logger.Trace(exception);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="exception"></param>
        public static void Debug(Exception exception)
        {
            Logger.Debug(exception);
        }

        /// <summary>
        /// 信息消息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// 信息消息
        /// </summary>
        /// <param name="exception"></param>
        public static void Info(Exception exception)
        {
            Logger.Info(exception);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message)
        {
            Logger.Warn(message);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="exception"></param>
        public static void Warn(Exception exception)
        {
            Logger.Warn(exception);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            Logger.Error(message);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="exception"></param>
        public static void Error(Exception exception)
        {
            Logger.Error(exception);
        }

        /// <summary>
        /// 非常严重的错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        /// <summary>
        /// 非常严重的错误
        /// </summary>
        /// <param name="exception"></param>
        public static void Fatal(Exception exception)
        {
            Logger.Fatal(exception);
        }

        /// <summary>
        /// 自定义日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="folder"></param>
        public static void Write(object message, string folder = "folder")
        {
            WriteLog(message, folder);
        }

        private static void WriteLog(object message, string folder)
        {
            var dt = DateTime.Now;
            // var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            lock (LogWriteLockObj)
            {
                try
                {
                    var dir = $@"{Environment.CurrentDirectory}\NLogs\{dt:yyyy-MM-dd}\{folder}";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    var path = Path.Combine(dir + @"\" + dt.ToString("yyyy-MM-dd HH") + ".log");
                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine("{0} | {1}", dt.ToString("yyyy-MM-dd HH:mm:ss"), message);
                        sw.Close();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}
