using Core.Exceptions;
using Chat;
using Logging;
using Shutdown;
using KeyValuePairDatabases;
using Chat.Messages.Client.Messages;
using DependencyManagement;
using Initialization.Exceptions;
using Configurations;

namespace Core.DAL
{
    public class DalLatestMessages
    {
        private static DalLatestMessages _Instance;
        public static DalLatestMessages Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalLatestMessages));
                return _Instance;
            }
        }
        public static DalLatestMessages Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalLatestMessages));
                _Instance = new DalLatestMessages();
                return _Instance;
        }
        private HashSet<LatestMessages> _ToFlushToCyclicalFile = new HashSet<LatestMessages>();
        private KeyValuePairInMemoryDatabase<long, LatestMessages>
            _LatestMessagesKeyValuePairInMemoryDatabase;
        private IdentifierLock<long> _IdentifierLock;
        private DirectoryInfo _CyclicalFilesRootDirectory;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private DalLatestMessages()
        {
            _CyclicalFilesRootDirectory = new DirectoryInfo(
                DependencyManager.GetString(DependencyNames.LatestMessagesCyclicalFilesRootDirectory).TrimEnd('\\', '/')
            );
            _IdentifierLock = new IdentifierLock<long>();
            _LatestMessagesKeyValuePairInMemoryDatabase
            = new KeyValuePairInMemoryDatabase<long, LatestMessages>(
                new OverflowParameters<long, LatestMessages>(overflowToNowhere:false,
                new NoIdentifierLock<long>(), 
                _OverflowEntry),
                new NoIdentifierLock<long>());
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.DalLatestMessages);
            _StartFlushThread();
        }
        public void Append(long conversationId, ClientMessage message)
        {
            LatestMessages latestMessages = _GetLatestMessages(conversationId);
            latestMessages.Append(message);
            lock (_ToFlushToCyclicalFile)
            {
                if (!_ToFlushToCyclicalFile.Contains(latestMessages))
                    _ToFlushToCyclicalFile.Add(latestMessages);
            }
        }
        public ClientMessage[] ReadAll(long conversationId)
        {
            return _GetLatestMessages(conversationId).AllMessages;
        }
        private LatestMessages _GetLatestMessages(long conversationId)
        {
            LatestMessages latestMessages = _LatestMessagesKeyValuePairInMemoryDatabase.Get(conversationId);
            if (latestMessages != null) return latestMessages;
            _IdentifierLock.LockForWrite(conversationId, () => {
                latestMessages = _LatestMessagesKeyValuePairInMemoryDatabase.Get(conversationId);
                if (latestMessages != null) return;
                latestMessages = new LatestMessages(Configurations.Lengths.OVERFLOWING_CHAR_HISTORY_LENGTH,
                    _GetCyclicalFilePath(conversationId));
                _LatestMessagesKeyValuePairInMemoryDatabase.Set(conversationId, latestMessages);
            });
            return latestMessages;
        }
        private string _GetCyclicalFilePath(long conversationId) {
            PathHelper.GetDirectoryPathAndFilePathFromIdentifier(
                conversationId.ToString(), out string directoryPath, out string filePath, 
                _CyclicalFilesRootDirectory, 2, ".bin");
            Directory.CreateDirectory(directoryPath);
            return filePath;
        }
        private void _OverflowEntry(long identifier, LatestMessages latestMessages) {
            latestMessages.FlushToCyclicalFile();
            lock (_ToFlushToCyclicalFile)
            {
                _ToFlushToCyclicalFile.Remove(latestMessages);
            }
        }
        public void Dispose() {
            if(_CancellationTokenSourceDisposed.IsCancellationRequested)return;
            _CancellationTokenSourceDisposed.Cancel();
            _FlushLatestMessagess();
        }
        private void _StartFlushThread() {
            new Thread(_FlushLooper).Start();
        }
        private void _FlushLooper() {
            while (!_CancellationTokenSourceDisposed.IsCancellationRequested)
            {
                int j = Delays.RECENT_MESSAGES_SECONDS_BETWEEN_FLUSHES / Delays.SLEEP_PERIOD_FLUSH_LOOPER_SECONDS;
                for (int i = 0; i < j; i++)
                {
                    Thread.Sleep(Delays.SLEEP_PERIOD_FLUSH_LOOPER_SECONDS);
                    if (_CancellationTokenSourceDisposed.IsCancellationRequested)
                        return;
                }
                _FlushLatestMessagess();
            }
        }
        private void _FlushLatestMessagess() {
            foreach (LatestMessages latestMessages in _TakeLatestMessagess())
            {
                try
                {
                    latestMessages.FlushToCyclicalFile();
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }
        }
        private LatestMessages[] _TakeLatestMessagess() {
            LatestMessages[] latestMessagess;
            lock (_ToFlushToCyclicalFile)
            {
                latestMessagess = _ToFlushToCyclicalFile.ToArray();
                _ToFlushToCyclicalFile.Clear();
            }
            return latestMessagess;
        }
    }
}