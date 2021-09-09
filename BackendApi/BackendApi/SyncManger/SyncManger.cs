using System.Threading.Tasks;
using BackendApi.Ulitis;

namespace BackendApi {
    public class SyncManger {
        private WaitPushPop<Point?> _iOTPoint;
        private WaitPushPop<Point?> _dbPoint;
        private int _delayTimeMs;  
        public SyncManger(int delayTimeMs) {
            _iOTPoint = new WaitPushPop<Point?>();
            _dbPoint = new WaitPushPop<Point?>();
            _delayTimeMs = delayTimeMs;
        }

        public static (SyncManger SyncManger, Task Task) FactoryStart(int delayTimeMs) {
            var syncManger = new SyncManger(delayTimeMs);
            return (syncManger, syncManger.Start());
        }

        public Task Start() => Task.Run(MoveLoop);
        
        public IWaitPush<Point?> GetIOTPointWaiter() => _iOTPoint;
        public IWaitPop<Point?> GetDbPointerWaiter() => _dbPoint;

        private Task MoveLoop() {
            var deltaTime = new DeltaTimeSleep(_delayTimeMs); 
            while (true) {
                _dbPoint.Push(_iOTPoint.Pop());
                deltaTime.Sleep();
            }
        }
    }
}