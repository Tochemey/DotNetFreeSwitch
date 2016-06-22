using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModFreeSwitch.Common {

    public class AsyncCountdownEvent {
        private readonly AsyncManualResetEvent _mAmre = new AsyncManualResetEvent();
        private int _mCount;
        /// <summary>
        ///     A countdown event is an event that will allow waiters to complete after receiving a particular number of signals.
        ///     The “countdown” comes from the common fork/join pattern in which it’s often utilized: a certain number of
        ///     operations participate, and as they complete they signal the event, which counts down from the original number to
        ///     0.  When it gets to 0, it becomes set, and all waiters can complete.
        /// </summary>
        public AsyncCountdownEvent(int initialCount) {
            if (initialCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialCount));
            _mCount = initialCount;
        }

        public Task WaitAsync() {
            return _mAmre.WaitAsync();
        }

        public void Signal() {
            if (_mCount <= 0)
                throw new InvalidOperationException();

            var newCount = Interlocked.Decrement(ref _mCount);
            if (newCount == 0)
                _mAmre.Set();
            else if (newCount < 0)
                throw new InvalidOperationException();
        }

        /// <summary>
        ///     One common usage of a type like AsyncCountdownEvent is using it as a form of a barrier: all participants signal and
        ///     then wait for all of the other participants to arrive.
        ///     Given that, we could also add a simple SignalAndWait method
        ///     to implement this common pattern
        /// </summary>
        /// <returns></returns>
        public Task SignalAndWait() {
            Signal();
            return WaitAsync();
        }
    }
}