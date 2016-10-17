using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBusDrivingSimulator.Core
{
    public static class Constants
    {
        public const int DEFAULT_SCREEN_WIDTH = 1280;
        public const int DEFAULT_SCREEN_HEIGHT = 720;

        public const string APPLICATION_NAME = "Open Bus Driving Simulator (OpenDBS)";
        public static readonly string VERSION_NUMBER = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

        public const string START_GAME = "Start Game";
    }
}
