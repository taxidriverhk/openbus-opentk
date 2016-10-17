using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBusDrivingSimulator.Core
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
        private static string logFilePath = "";
        private static string logFileName = "";

        private static string logLineFormat()
        {
            return "";
        }

        public static void Write(LogLevel level, string format, params object[] variables)
        {

        }
    }
}
