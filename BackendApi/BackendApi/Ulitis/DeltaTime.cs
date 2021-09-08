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

        public async Task Sleep() {
            var timeNow = DateTime.Now.Ticks;
            var sleep = WaitTimeMs - (int)((timeNow - LastWait) / TimeSpan.TicksPerMillisecond);
            await Task.Delay(sleep);

            LastWait = DateTime.Now.Ticks;
        }
    }
}