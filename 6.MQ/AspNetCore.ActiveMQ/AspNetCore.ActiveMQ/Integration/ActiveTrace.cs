using System;

namespace AspNetCore.ActiveMQ.Integration
{
    public enum TraceLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        Fatal = 4
    }
    public class ActiveTrace : Apache.NMS.ITrace
    {
        TraceLevel traceLevel;
        Action<TraceLevel, string> action;
        public ActiveTrace(TraceLevel traceLevel, Action<TraceLevel, string> action)
        {
            this.traceLevel = traceLevel;
            this.action = action;
        }


        public bool IsDebugEnabled => traceLevel <= TraceLevel.Debug;

        public bool IsInfoEnabled => traceLevel <= TraceLevel.Info;

        public bool IsWarnEnabled => traceLevel <= TraceLevel.Warn;

        public bool IsErrorEnabled => traceLevel <= TraceLevel.Error;

        public bool IsFatalEnabled => traceLevel <= TraceLevel.Fatal;

        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                action?.Invoke(TraceLevel.Debug, message);
            }
        }

        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                action?.Invoke(TraceLevel.Error, message);
            }
        }

        public void Fatal(string message)
        {
            if (IsFatalEnabled)
            {
                action?.Invoke(TraceLevel.Fatal, message);
            }
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                action?.Invoke(TraceLevel.Info, message);
            }
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
            {
                action?.Invoke(TraceLevel.Warn, message);
            }
        }
    }
}
