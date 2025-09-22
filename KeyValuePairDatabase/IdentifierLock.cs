using Core;
using Core.Locks;
using Core.Timing;
using Logging;

namespace KeyValuePairDatabases
{
    public class IdentifierLock<TIdentifier> : IIdentifierLock<TIdentifier>
    {
        private const int TIMEOUT_WAIT = 3000;
        private class LockInstance
        {
            private CountdownLatch _CountdownLatch = new CountdownLatch();
            private CancellationTokenSource _CancellationToken = new CancellationTokenSource();
            public bool ReadElseWrite { get; }

#if DEBUG_IDENTIFIER_LOCKS
            public Dictionary<Thread, string> ThreadsSeen = new Dictionary<Thread, string>();
            public volatile int TotalIncrements = 0;
            public volatile int TotalDecrements = 0;
            public ThreadState[] ThreadStatuses { get { return ThreadsSeen.GroupBy(t=>t.ThreadState).Select(g=>g.First().ThreadState).ToArray(); } }
            public string StackTrace { get; }
            public long ThreadId { get; }
            long BeenGoingFor { get { return TimeHelper.MillisecondsNow - CreatedAt; } }
            public long CreatedAt { get; }
#endif

            private Action _CallbackRemove;
            public LockInstance(bool readElseWrite, Action callbackRemove) {
                ReadElseWrite = readElseWrite;
                _CallbackRemove = callbackRemove;

#if DEBUG_IDENTIFIER_LOCKS
                StackTrace = System.Environment.StackTrace;
                ThreadId = Thread.CurrentThread.ManagedThreadId;
                CreatedAt = TimeHelper.MillisecondsNow;
                ThreadsSeen.Add(Thread.CurrentThread, StackTrace);
#endif

            }
            public bool Wait() {
                if(_CancellationToken.IsCancellationRequested) return false;
                return _CountdownLatch.Wait(TIMEOUT_WAIT, _CancellationToken.Token);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Was first cancel</returns>
            public bool Cancel() {
                bool wasAlreadyCancelled = _CancellationToken.IsCancellationRequested;
                if (wasAlreadyCancelled) return false;
                _CancellationToken.Cancel();
                _CallbackRemove();
                return true;
            }
            public void IncrementCountdownLatchCount()
            {
#if DEBUG_IDENTIFIER_LOCKS
                lock (ThreadsSeen)
                {
                    ThreadsSeen.Add(Thread.CurrentThread, System.Environment.StackTrace);
                }
                TotalIncrements++;
#endif
                _CountdownLatch.Increment();
            }
            public void DecrementCountdownLatchCount()
            {

#if DEBUG_IDENTIFIER_LOCKS
                lock (ThreadsSeen)
                {
                    ThreadsSeen.Remove(Thread.CurrentThread);
                }
                TotalDecrements++;
#endif
                _CountdownLatch.Signal();
                if (_CountdownLatch.Count < 1)
                {
                    _CallbackRemove();
                }
            }
        }
        private Dictionary<TIdentifier, LockInstance> _MapIdentifierToLockInstance = new Dictionary<TIdentifier, LockInstance>();
        public TReadResult LockForReads<TReadResult>(TIdentifier identifier, Func<TReadResult> callback)
        {
            while (true)
            {
                LockInstance lockInstance = GetExistingLockInstanceOrCreateMyLockInstance(identifier,
                out bool mine, isReadElseWrite: true);
                if (mine || lockInstance.ReadElseWrite)
                {
                    try
                    {
                        return callback();
                    }
                    finally
                    {
                        lock (_MapIdentifierToLockInstance)
                        {
                            lockInstance.DecrementCountdownLatchCount();
                        }
                    }
                }
                WaitHandlingTimeout(lockInstance);
            }
        }
        public void LockForReads(TIdentifier identifier, Action callback)
        {
            while (true)
            {
                LockInstance lockInstance = GetExistingLockInstanceOrCreateMyLockInstance(identifier,
                out bool mine, isReadElseWrite: true);
                if (mine || lockInstance.ReadElseWrite)
                {
                    try
                    {
                        callback();
                        return;
                    }
                    finally
                    {
                        lock (_MapIdentifierToLockInstance)
                        {
                            lockInstance.DecrementCountdownLatchCount();
                        }
                    }
                }
                WaitHandlingTimeout(lockInstance);
            }
        }
        public void LockForWrite(TIdentifier identifier, Action callback)
        {
            while (true)
            {
                LockInstance lockInstance = GetExistingLockInstanceOrCreateMyLockInstance(identifier,
                out bool mine, isReadElseWrite: false);
                if (mine)
                {
                    try
                    {
                        callback();
                        return;
                    }
                    finally
                    {
                        lock (_MapIdentifierToLockInstance)
                        {
                            lockInstance.DecrementCountdownLatchCount();
                        }
                    }
                }
                WaitHandlingTimeout(lockInstance);
            }
        }
        private LockInstance GetExistingLockInstanceOrCreateMyLockInstance(TIdentifier identifier, out bool mine,
            bool isReadElseWrite)
        {

            lock (_MapIdentifierToLockInstance)
            {
                if (_MapIdentifierToLockInstance.TryGetValue(identifier, out LockInstance lockInstance))
                {
                    if (lockInstance.ReadElseWrite)//THIS WAS THE CAUSE OF THE HORRIBLE BUG. it was isReadElseWrite
                        lockInstance.IncrementCountdownLatchCount();
                    mine = false;
                    return lockInstance;
                }
                lockInstance = new LockInstance(isReadElseWrite, () =>
                {
                    if (_MapIdentifierToLockInstance.TryGetValue(identifier, out LockInstance currentLockInstance)) {
                        if (currentLockInstance.Equals(lockInstance))
                        {
                            _MapIdentifierToLockInstance.Remove(identifier);
                        }
                        else { 
                        
                        }
                    }
                });
                _MapIdentifierToLockInstance[identifier] = lockInstance;
                mine = true;
                return lockInstance;
            }
        }
        private void WaitHandlingTimeout(LockInstance lockInstance) {

            if (lockInstance.Wait()) return;
            lock (_MapIdentifierToLockInstance)
            {
                if (lockInstance.Cancel())
                {
                    Logs.HighPriority.Error(new Exception("Watched lock was alive too long! " + System.Environment.StackTrace));
                }
            }
        }//Why is it always one that cant complete on time????
        //Just one entry into that lock. Something isnt right. 
    }
}