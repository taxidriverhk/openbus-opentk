using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenBus.Core
{
    public enum LogLevel
    {
        ERROR,
        WARN,
        INFO,
        DEBUG,
        TRACE
    }

    public static class Log
    {
        private static string logFilePath;
        private static readonly string logLineFormat = "{0} - {1} - {2}{3}";

        static Log()
        {
            logFilePath = GameEnvironment.RootPath 
                + string.Format("OpenBDS_Log_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        public static void Write(LogLevel level, string format, params object[] variables)
        {
            string message = string.Format(format, variables);
            File.AppendAllText(logFilePath, string.Format(logLineFormat, 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), level.ToString(), message, Environment.NewLine));
        }
    }
}
