using InfernoDispatcher;
using KeyValuePairDatabases.Enums;

namespace KeyValuePairDatabases
{
    internal class IdentifierLock_LockInstance
    {
        private List<IdentifierLock_ToRun> _ScheduledReads;
        private LinkedList<IdentifierLock_ToRun> _ScheduledWrites;
        private IdentiferLockState _State = IdentiferLockState.NotRunning;
        private int _NReadsRunning;
        private readonly object _LockObject;
        private readonly Action _Remove;
        public IdentifierLock_LockInstance(object lockObject, Action remove)
        {
            _Remove = remove;
            _LockObject = lockObject;
        }
        public void ScheduleRead(IdentifierLock_ToRun toRun)
        {
            lock (_LockObject)
            {
                switch (_State)
                {
                    case IdentiferLockState.NotRunning:
                    case IdentiferLockState.DoingReads:
                        _State = IdentiferLockState.DoingReads;
                        _NReadsRunning++;
                        try
                        {
                            Dispatcher.Instance.Run(() => toRun.Run(DoneRead));
                        }
                        catch
                        {
                            DoneReadUnlocked();
                            throw;
                        }
                        break;
                    case IdentiferLockState.DoingWrite:
                        if (_ScheduledReads == null) _ScheduledReads = new List<IdentifierLock_ToRun> { toRun };
                        else _ScheduledReads.Add(toRun);
                        break;
                    default:
                        throw new NotImplementedException($"Was not implemented for {nameof(IdentiferLockState)}.{Enum.GetName(typeof(IdentiferLockState), _State)}");

                }
            }
        }
        public void ScheduleWrite(IdentifierLock_ToRun toRun)
        {
            lock (_LockObject)
            {
                switch (_State)
                {
                    case IdentiferLockState.NotRunning:
                        _State = IdentiferLockState.DoingWrite;
                        try
                        {
                            Dispatcher.Instance.Run(() => toRun.Run(DoneWrite));
                        }
                        catch
                        {
                            DoneWriteUnlocked();
                            throw;
                        }
                        break;
                    case IdentiferLockState.DoingWrite:
                    case IdentiferLockState.DoingReads:
                        if (_ScheduledWrites == null) _ScheduledWrites = new LinkedList<IdentifierLock_ToRun>();
                        _ScheduledWrites.AddLast(toRun);
                        break;
                    default:
                        throw new NotImplementedException($"Was not implemented for {nameof(IdentiferLockState)}.{Enum.GetName(typeof(IdentiferLockState), _State)}");

                }
            }
        }
        /// <summary>
        /// Do next write, else do reads, else remove
        /// </summary>
        private void DoneWrite()
        {
            lock (_LockObject)
            {
                DoneWriteUnlocked();
            }
        }
        private void DoneWriteUnlocked()
        {

            IdentifierLock_ToRun? nextWrite;
            if (_ScheduledWrites != null && (nextWrite = _ScheduledWrites.FirstOrDefault()) != null)
            {
                _ScheduledWrites.RemoveFirst();
                try
                {
                    Dispatcher.Instance.Run(() => nextWrite!.Run(DoneWrite));
                }
                catch (Exception ex)
                {
                    nextWrite.Failed(ex);
                    DoneWriteUnlocked();
                }
                return;
            }
            if (_ScheduledReads != null && _ScheduledReads.Any())
            {
                _NReadsRunning = _ScheduledReads.Count;
                _State = IdentiferLockState.DoingReads;
                var scheduledReads = _ScheduledReads;
                _ScheduledReads = null;
                foreach (var toRun in scheduledReads)
                {
                    try
                    {
                        Dispatcher.Instance.Run(() => toRun.Run(DoneRead));
                    }
                    catch (Exception ex)
                    {
                        toRun.Failed(ex);
                        DoneReadUnlocked();
                    }
                }
                return;
            }
            _Remove();
        }
        /// <summary>
        /// Decrement NRunning, check if done all reads and if so do writes if there are any else remove.
        /// </summary>
        private void DoneRead()
        {
            lock (_LockObject)
            {
                DoneReadUnlocked();
            }
        }
        private void DoneReadUnlocked()
        {

            _NReadsRunning--;
            if (_NReadsRunning > 0) return;
            IdentifierLock_ToRun? nextWrite;
            if (_ScheduledWrites != null && (nextWrite = _ScheduledWrites.FirstOrDefault()) != null)
            {
                _State = IdentiferLockState.DoingWrite;
                _ScheduledWrites.RemoveFirst();
                try
                {
                    Dispatcher.Instance.Run(() => nextWrite!.Run(DoneWrite));
                }
                catch (Exception ex)
                {
                    nextWrite.Failed(ex);
                    DoneWriteUnlocked();
                }
                return;
            }
            _Remove();
        }
    }
}