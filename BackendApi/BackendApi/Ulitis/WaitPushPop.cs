using System.Threading.Tasks;

namespace BackendApi.Ulitis {
    public class WaitPushPop<T> : IWaitPush<T>, IWaitPop<T> {
        private bool allowPop;
        private T? _value;
        private const int DelayTime = 100;
        public void Push(T value) {
            while (allowPop) Task.Delay(DelayTime).Wait();

            allowPop = true;
            _value = value;
        }

        public async Task PushAsync(T value) {
            while (allowPop) await Task.Delay(DelayTime);

            allowPop = true;
            _value = value;
        }

        public T? Pop() {
            while (!allowPop) Task.Delay(DelayTime).Wait();

            var value = _value;
            _value = default;
            allowPop = false;
            return value;
        }

        public async Task<T?> PopAsync() {
            while (!allowPop) await Task.Delay(DelayTime);
            var value = _value;
            _value = default;
            allowPop = false;
            return value;
        }

        public bool CanPush() => !allowPop;

        public bool CanPop() => allowPop;

        public IWaitPush<T> GetPushOnly() => this;

        public IWaitPop<T> GetPopOnly() => this;
    }

    public interface IWaitPush<T> {
        public void Push(T value);
        public bool CanPush();
        public Task PushAsync(T value);
    }
    
    public interface IWaitPop<T> {
        public T? Pop();
        public Task<T?> PopAsync();
        public bool CanPop();
    }
}