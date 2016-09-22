using System;
using System.Diagnostics;

namespace OpenBusDrivingSimulator.Core
{
    public class Timer
    {
        private Stopwatch stopWatch;
        private long lastTime;

        public static Timer Initialize()
        {
            Timer timer = new Timer();
            timer.stopWatch = new Stopwatch();
            timer.Reset();
            return timer;
        }

        public void Reset()
        {
            this.stopWatch.Start();
            this.lastTime = this.stopWatch.ElapsedMilliseconds;
        }

        public double TimeElapsed
        {
            get
            {
                long currentTime = this.stopWatch.ElapsedMilliseconds;
                long diff = currentTime - this.lastTime;
                this.lastTime = currentTime;
                return diff * 0.001;
            }
        }
    }
}
