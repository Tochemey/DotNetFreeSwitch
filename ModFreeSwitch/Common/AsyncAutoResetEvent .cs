using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModFreeSwitch.Common {
    public class AsyncAutoResetEvent {
        private static readonly Task SCompleted = Task.FromResult(true);

        private readonly Queue<TaskCompletionSource<bool>> _mWaits =
            new Queue<TaskCompletionSource<bool>>();

        private bool m_signaled;

        public Task WaitAsync() {
            lock (_mWaits) {
                if (m_signaled) {
                    m_signaled = false;
                    return SCompleted;
                }
                var tcs = new TaskCompletionSource<bool>();
                _mWaits.Enqueue(tcs);
                return tcs.Task;
            }
        }

        public void Set() {
            TaskCompletionSource<bool> toRelease = null;
            lock (_mWaits) {
                if (_mWaits.Count > 0)
                    toRelease = _mWaits.Dequeue();
                else if (!m_signaled)
                    m_signaled = true;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}