using Microsoft.Extensions.Logging;
using System;

namespace FootStone.Core
{
    public class NLogger : ILogger
    {
        private NLog.Logger logger = NLog.LogManager.GetLogger("Orleans");

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            NLog.LogLevel level = NLog.LogLevel.Off;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    level = NLog.LogLevel.Trace;
                    break;
                case LogLevel.Debug:
                    level = NLog.LogLevel.Debug;
                    break;
                case LogLevel.Information:
                    level = NLog.LogLevel.Info;
                    break;
                case LogLevel.Warning:
                    level = NLog.LogLevel.Warn;
                    break;
                case LogLevel.Error:
                    level = NLog.LogLevel.Error;
                    break;
                case LogLevel.Critical:
                    level = NLog.LogLevel.Fatal;
                    break;
                case LogLevel.None:
                    level = NLog.LogLevel.Off;
                    break;
            }
            logger.Log(level, formatter(state, exception));
        }
    }

    public class NLoggerProvider : ILoggerProvider
    {
        public NLoggerProvider()
        {
        }
        
        public void Dispose()
        {
        }

        ILogger ILoggerProvider.CreateLogger(string categoryName)
        {
            return new NLogger();
        }
    }

    /*************************************************************************************/

    public class NLogice : Ice.Logger
    {
        private NLog.Logger logger;

        public string Prefix { get; set; }


        public NLogice(NLog.Logger logger)
        {
            this.logger = logger;
        }

        public void print(string message)
        {
            logger.Info(message);
        }

        public void trace(string category, string message)
        {
            logger.Trace(message);
        }

        public void warning(string message)
        {
            logger.Warn(message);
        }

        public void error(string message)
        {
            logger.Error(message);
        }

        public string getPrefix()
        {
            return Prefix;
        }

        public Ice.Logger cloneWithPrefix(string prefix)
        {
            return null;
        }

        public void debug(string message)
        {
            logger.Debug(message);
        }
    }
}
