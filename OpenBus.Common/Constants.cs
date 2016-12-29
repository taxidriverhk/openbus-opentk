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
        // GUI constants
        public const int DEFAULT_SCREEN_WIDTH = 1280;
        public const int DEFAULT_SCREEN_HEIGHT = 720;

        // Config constants
        public const char PATH_DELIM = '\\';

        // System-wide constants
        public const string APPLICATION_NAME = "Open Bus Driving Simulator (OpenBus)";
        public const string APPLICATION_ICON = "favicon.ico";
        public static string VERSION_NUMBER
        {
            get
            {
                Version version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                string versionString = string.Format("{0}.{1:00}.{2}.{3:00000}", 
                    version.Major, version.Minor, version.Build, version.Revision);
                #if DEBUG
                versionString += " Debug Version";
                #endif
                return versionString;
            }
        }
    }
}
