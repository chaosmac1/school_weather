namespace BackendApi.Ulitis; 

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
            
        var diffMs = (int)((timeNow - _lastWait) / TimeSpan.TicksPerMillisecond);
        var sleepMs = _waitTimeMs - diffMs;
            
        if (sleepMs < 0) {
            _lastWait = DateTime.Now.Ticks;
            return;
        }
            
        Task.Delay(sleepMs).Wait();
        _lastWait = DateTime.Now.Ticks;
    }
}