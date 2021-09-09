using System;
using System.Threading.Tasks;

namespace BackendApi.Ulitis {
    public ref struct DeltaTimeSleep {
        private long _lastWait;
        private readonly int _waitTimeMs;

        public DeltaTimeSleep(int waitTimeMs) {
            _lastWait = DateTime.Now.Ticks;
            _waitTimeMs = waitTimeMs;
        }

        public DeltaTimeSleep(long waitTimeTicks) {
            _lastWait = DateTime.Now.Ticks;
            _waitTimeMs = (int)(waitTimeTicks / TimeSpan.TicksPerMillisecond);
        }

        public void Sleep() {
            var timeNow = DateTime.Now.Ticks;
            var sleep = _waitTimeMs - (int)((timeNow - _lastWait) / TimeSpan.TicksPerMillisecond);
            Task.Delay(sleep).Wait();
            _lastWait = DateTime.Now.Ticks;
        }
    }
}