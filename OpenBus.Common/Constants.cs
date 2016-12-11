using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBus.Common
{
    public static class EnvironmentVariables
    {
#if DEBUG
        public static string RootPath = @"D:\Downloads\OpenBus\";
#else
        public static string RootPath = System.IO.Directory.GetCurrentDirectory() + "\\";
#endif
        public static string MapPath = RootPath + "maps";
    }

    public static class Constants
    {
        public const int DEFAULT_SCREEN_WIDTH = 1280;
        public const int DEFAULT_SCREEN_HEIGHT = 720;

        public const char PATH_DELIM = '\\';

        public const string APPLICATION_NAME = "Open Bus Driving Simulator (OpenBus)";
        public const string APPLICATION_ICON = "favicon.ico";
        public static readonly string VERSION_NUMBER = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

        public const string START_GAME = "Start Game";
    }
}
