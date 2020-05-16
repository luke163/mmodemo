
namespace FootStone
{
    public class NLogger
    {
        private static ILogger logger;

        public static void Init(ILogger tmp)
        {
            logger = tmp;
        }

        public static void Trace(string message)
        {
            if(logger != null) logger.Trace(message);
        }

        public static void Debug(string message)
        {
            if (logger != null) logger.Debug(message);
        }

        public static void Info(string message)
        {
            if (logger != null) logger.Info(message);
        }

        public static void Warn(string message)
        {
            if (logger != null) logger.Warn(message);
        }

        public static void Error(string message)
        {
            if (logger != null) logger.Error(message);
        }
        public static void Fatal(string message)
        {
            if (logger != null) logger.Fatal(message);
        }
    }

    public interface ILogger
    {
        void Trace(string message);

        void Debug(string message);

        void Info(string message);

        void Warn(string message);

        void Error(string message);

        void Fatal(string message);
    }

    /*************************************************************************************/

    public class NLogice : Ice.Logger
    {
        private string prefix;

        public NLogice(string pre = "Ice")
        {
            prefix = pre;
        }

        public void print(string message)
        {
            NLogger.Info(message);
        }

        public void trace(string category, string message)
        {
            NLogger.Trace(message);
        }

        public void warning(string message)
        {
            NLogger.Warn(message);
        }

        public void error(string message)
        {
            NLogger.Error(message);
        }

        public string getPrefix()
        {
            return prefix;
        }
        public Ice.Logger cloneWithPrefix(string prefix)
        {
            return null;
        }
    }
}
