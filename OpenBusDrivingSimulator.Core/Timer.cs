using System;
using System.Diagnostics;

namespace OpenBusDrivingSimulator.Core
{
    public static class Timer
    {
        private static Stopwatch timer;
        private static long lastTime;

        public static void Initialize()
        {
            timer = new Stopwatch();
            timer.Start();
            lastTime = timer.ElapsedMilliseconds;
        }

        public static double TimeElapsed
        {
            get
            {
                long currentTime = timer.ElapsedMilliseconds;
                long diff = currentTime - lastTime;
                lastTime = currentTime;
                return diff * 0.001;
            }
        }
    }
}
