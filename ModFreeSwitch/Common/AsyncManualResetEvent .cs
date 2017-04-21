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

using System.Threading;
using System.Threading.Tasks;

namespace ModFreeSwitch.Common
{
    public class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> _mTcs = new TaskCompletionSource<bool>();

        public Task WaitAsync() { return _mTcs.Task; }

        public void Set()
        {
            var tcs = _mTcs;
            Task.Factory.StartNew(s => ((TaskCompletionSource<bool>) s).TrySetResult(true),
                tcs,
                CancellationToken.None,
                TaskCreationOptions.PreferFairness,
                TaskScheduler.Default);
            tcs.Task.Wait();
        }

        public void Reset()
        {
            while (true)
            {
                var tcs = _mTcs;
                var taskCompletionSource = _mTcs;
                if (!tcs.Task.IsCompleted || Interlocked.CompareExchange(ref taskCompletionSource,
                        new TaskCompletionSource<bool>(),
                        tcs) == tcs) return;
            }
        }
    }
}