using Core.Collections;
using Core.Exceptions;
using Core.MemoryManagement;
using DependencyManagement;
using KeyValuePairDatabases;
using Logging;

namespace MultimediaServerCore
{
    public sealed class MultimediaCache : IMemoryManaged
    {
        private static MultimediaCache? _Instance;
        private OrderedDictionary<string, MultimediaCacheEntry> _MapMultimediaTokenToMultimediaCacheEntry
            = new OrderedDictionary<string, MultimediaCacheEntry>(e=>e.Path);
        private IdentifierLock<string> _IdentifierLockCreate = new IdentifierLock<string>();
        private long _Size;
        public static MultimediaCache Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MultimediaCache));
            _Instance = new MultimediaCache();
            return _Instance;
        }
        public static MultimediaCache Instance
        {
            get
            {
                if (_Instance == null) 
                    throw new NotInitializedException(nameof(MultimediaCache));
                return _Instance;
            }
        }
        private MultimediaCache() {
            MemoryManager.Instance.Add(this);
        }
        public byte[]? Get(string path, out string contentType) {
            MultimediaCacheEntry? entry = null;
            contentType = null;
            lock (_MapMultimediaTokenToMultimediaCacheEntry) {
                if (_MapMultimediaTokenToMultimediaCacheEntry.TryGetValue(path, out entry)) {
                    _MapMultimediaTokenToMultimediaCacheEntry.AppendOrMoveToLast(entry);
                }
            }
            if (entry != null)
            {
                contentType = entry.ContentType;
                return entry.Bytes;
            }
            _IdentifierLockCreate.LockForWrite(path, () =>
            {
                lock (_MapMultimediaTokenToMultimediaCacheEntry)
                {
                    if (_MapMultimediaTokenToMultimediaCacheEntry.TryGetValue(path, out entry))
                        return;
                }
                byte[]? bytes = ReadEntryBytes(path, out string contentTypeInternal);
                if(bytes==null) return;
                entry = new MultimediaCacheEntry(path, bytes, contentTypeInternal);
                lock (_MapMultimediaTokenToMultimediaCacheEntry)
                {
                    _MapMultimediaTokenToMultimediaCacheEntry.AppendOrMoveToLast(entry);
                    _Size += entry.Size;
                }
            });
            if (entry == null)
                return null;
            contentType = entry.ContentType;
            return entry.Bytes;
        }
        private byte[]? ReadEntryBytes(string path, out string contentType) {
            Logs.Default.Info("ReadEntryBytes");
            if (Path.DirectorySeparatorChar == '\\')
                path = path.Replace('/', '\\');
            Logs.Default.Info("path was " + path);
            string filePath = Path.Combine(DependencyManager.GetString(DependencyNames.MultimediaDirectory), path);
            Logs.Default.Info("filePath was " + filePath);
            contentType = MimeTypes.MimeTypeMap.GetMimeType(filePath);
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch {
                return null;
            }
        }

        public void ReduceMemoryFootprintByProportion(float proportion, CancellationToken? cancellationToken)
        {
            long desiredSize = (long)Math.Floor(proportion * _Size);/*
            if (desiredSize <= 0) {
                lock (_MapMultimediaTokenToMultimediaCacheEntry) {
                    //Incase error causes negative number
                    _MapMultimediaTokenToMultimediaCacheEntry.Clear();
                }
            }*/
            lock (_MapMultimediaTokenToMultimediaCacheEntry) {
                _MapMultimediaTokenToMultimediaCacheEntry.TakeFromFirstWhile((entry) => {
                    _Size -= entry.Size;
                    return _Size > desiredSize;
                });
            }
        }
    }
}