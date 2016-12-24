using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace OpenBus.Common
{
    public enum LogLevel
    {
        Error,
        Warn,
        Info,
        Trace,
        Debug
    }

    public static class Log
    {
        private const int LOG_LIST_THRESHOLD = 100;
        private const string LOG_FILE_FORMAT = "OpenBDS_Log_{0}.txt";
        private const string LOG_LINE_FORMAT = "{0} - {1} - {2}";

        private static List<string> logs;
        private static Thread logWriteThread;

        private static LogLevel lowestLevel;
        private static string logFilePath;

        static Log()
        {
            lowestLevel = LogLevel.Debug;
            logFilePath = EnvironmentVariables.RootPath 
                + string.Format(LOG_FILE_FORMAT, DateTime.Now.ToString("yyyy-MM-dd"));
            logs = new List<string>();
        }

        public static void Write(LogLevel level, string format, params object[] variables)
        {
            string message = string.Format(format, variables);
            if (level <= lowestLevel)
                logs.Add(string.Format(LOG_LINE_FORMAT, 
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                    level.ToString(), message));
            if (logs.Count > LOG_LIST_THRESHOLD)
                RunLogWriteThread();
        }

        private static void RunLogWriteThread()
        {
            if (logWriteThread == null || 
                logWriteThread.ThreadState != ThreadState.Running)
            {
                logWriteThread = new Thread(delegate ()
                {
                    File.AppendAllLines(logFilePath, logs);
                    logs.Clear();
                });
                logWriteThread.IsBackground = true;
                logWriteThread.Start();
            }
        }
    }
}
