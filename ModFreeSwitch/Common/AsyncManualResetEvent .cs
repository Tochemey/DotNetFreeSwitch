using System.Threading;
using System.Threading.Tasks;

namespace ModFreeSwitch.Common {
    public class AsyncManualResetEvent {
        private volatile TaskCompletionSource<bool> _mTcs =
            new TaskCompletionSource<bool>();

        public Task WaitAsync() {
            return _mTcs.Task;
        }

        public void Set() {
            var tcs = _mTcs;
            Task.Factory.StartNew(
                s => ((TaskCompletionSource<bool>) s).TrySetResult(true),
                tcs,
                CancellationToken.None,
                TaskCreationOptions.PreferFairness,
                TaskScheduler.Default);
            tcs.Task.Wait();
        }

        public void Reset() {
            while (true) {
                var tcs = _mTcs;
                TaskCompletionSource<bool> taskCompletionSource = _mTcs;
                if (!tcs.Task.IsCompleted ||
                    Interlocked.CompareExchange(ref taskCompletionSource,
                        new TaskCompletionSource<bool>(),
                        tcs) == tcs)
                    return;
            }
        }
    }
}