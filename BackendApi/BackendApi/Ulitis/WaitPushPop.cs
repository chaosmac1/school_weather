using System.Data.SqlTypes;
using System.Threading.Tasks;

namespace BackendApi.Ulitis {
    public struct WaitPushPop<T> : IWaitPush<T>, IWaitPop<T> {
        private bool _allowPop;
        private T? _value;

        public void Push(T value) {
            while (_allowPop) {
                Task.Delay(10).Wait();
            }

            _allowPop = true;
            _value = value;
        }

        public async Task PushAsync(T value) {
            while (_allowPop) {
                await Task.Delay(10);
            }

            _allowPop = true;
            _value = value;
        }

        public T? Pop() {
            while (!_allowPop) {
                Task.Delay(10).Wait();
            }

            var value = _value;
            _value = default;
            _allowPop = false;
            return value;
        }

        public bool CanPush() => !_allowPop;
        public bool CanPop() => _allowPop;

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