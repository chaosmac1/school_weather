using System;
using System.Threading.Tasks;

namespace BackendApi.Ulitis {
    public ref struct DeltaTimeSleep {
        private long LastWait;
        private int WaitTimeMs;

        public DeltaTimeSleep(int waitTimeMs) {
            LastWait = DateTime.Now.Ticks;
            WaitTimeMs = waitTimeMs;
        }

        public DeltaTimeSleep(long waitTimeTicks) {
            LastWait = DateTime.Now.Ticks;
            WaitTimeMs = (int)(waitTimeTicks / TimeSpan.TicksPerMillisecond);
        }

        public void Sleep() {
            var timeNow = DateTime.Now.Ticks;
            var sleep = WaitTimeMs - (int)((timeNow - LastWait) / TimeSpan.TicksPerMillisecond);
            Task.Delay(sleep).Wait();
            LastWait = DateTime.Now.Ticks;
        }
    }
}