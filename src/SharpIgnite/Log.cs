using log4net;
using log4net.Config;
using System.Reflection;

namespace SharpIgnite
{
    public static class Log
    {
        static ILog log;

        static Log()
        {
            XmlConfigurator.Configure();
            log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public static void Error(string message)
        {
            log.Error(message);
        }

        public static void Debug(string message)
        {
            log.Debug(message);
        }

        public static void Info(string message)
        {
            log.Info(message);
        }
    }
}
