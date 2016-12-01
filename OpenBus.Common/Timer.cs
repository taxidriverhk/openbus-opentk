using System;
using System.Diagnostics;

namespace OpenBus.Common
{
    public static class Timer
    {
        private static Stopwatch stopWatch;
        private static long lastTime;

        public static void Start()
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
            lastTime = stopWatch.ElapsedMilliseconds;
        }

        public static double TimeElapsed
        {
            get
            {
                long currentTime = stopWatch.ElapsedMilliseconds;
                long diff = currentTime - lastTime;
                lastTime = currentTime;
                return diff * 0.001;
            }
        }
    }
}
