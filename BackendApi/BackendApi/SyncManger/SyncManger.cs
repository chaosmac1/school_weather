using System;
using System.Threading.Tasks;
using BackendApi.Ulitis;

#nullable enable

namespace BackendApi {
    public class SyncManger {
        private WaitPushPop<Point?> _dbPoint;
        private readonly int _delayTimeMs;
        private WaitPushPop<Point?> _iOtPoint;
        private bool _taskRun;
        public SyncManger(int delayTimeMs) {
            _iOtPoint = new WaitPushPop<Point?>();
            _dbPoint = new WaitPushPop<Point?>();
            _delayTimeMs = delayTimeMs;
            _taskRun = false;
        }

        public static (SyncManger SyncManger, Task Task) FactoryStart(int delayTimeMs) {
            var syncManger = new SyncManger(delayTimeMs);
            return (syncManger, syncManger.Start());
        }

        public Task Start() {
            if (_taskRun) throw new Exception("Task Is Running");
            _taskRun = true;
            return Task.Run(MoveLoop);
        }

        public IWaitPush<Point?> GetIotPointWaiter() => _iOtPoint;

        public IWaitPop<Point?> GetDbPointerWaiter() => _dbPoint;

        private void MoveLoop() {
            var deltaTime = new DeltaTimeSleep(_delayTimeMs);
            while (true) {
                _dbPoint.Push(_iOtPoint.Pop());
                deltaTime.Sleep();
            }
        }
    }
}