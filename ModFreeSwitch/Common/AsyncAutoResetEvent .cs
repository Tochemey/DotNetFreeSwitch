/*
    Copyright [2016] [Arsene Tochemey GANDOTE]

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

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
