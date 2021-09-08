using System.Data.SqlTypes;
using System.Threading.Tasks;

namespace BackendApi.Ulitis {
    public struct WaitPushPop<T> : IWaitPush<T>, IWaitPop<T> {
        private bool _allowPush;
        private T? _value;

        public void Push(T value) {
            while (!_allowPush) {
                Task.Delay(10).Wait();
            }

            _allowPush = false;
            _value = value;
        }

        public async Task PushAsync(T value) {
            while (!_allowPush) {
                await Task.Delay(10);
            }

            _allowPush = false;
            _value = value;
        }

        public T? Pop() {
            while (_allowPush) {
                Task.Delay(10).Wait();
            }

            var value = _value;
            _value = default;
            _allowPush = true;
            return value;
        }

        public bool CanPush() => _allowPush;
        public bool CanPop() => !_allowPush;

        public IWaitPush<T> GetPushOnly() => this;
        public IWaitPop<T> GetPopOnly() => this;
    }

    public interface IWaitPush<T> {
        public void Push(T value); 
        public bool CanPush();
        public Task PushAsync(T value);
    }
    public interface IWaitPop<T> { public T? Pop(); public bool CanPop(); }
}