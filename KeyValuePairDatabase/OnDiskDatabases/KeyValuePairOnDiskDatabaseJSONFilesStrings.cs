
using Core.FileSystem;
using Core.Delegates;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases;
using PathHelper = KeyValuePairDatabases.PathHelper;
namespace KeyValuePairDatabases.OnDiskDatabases
{
    public class KeyValuePairOnDiskDatabaseJSONFilesStrings<TIdentifier> : IKeyValuePairOnDiskDatabase<TIdentifier, string>
    {
        private DirectoryInfo _DirectoryInfoRoot;
        private int _NCharactersEachLevel;
        private string _Extension;
        private IIdentifierLock<TIdentifier> _IdentifierBasedFileLock;
        private IdentifierLock<string> _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS = new IdentifierLock<string>();
        public KeyValuePairOnDiskDatabaseJSONFilesStrings(string rootDirectory, int nCharactersEachLevel,
            string extension, IIdentifierLock<TIdentifier> identifierLock)
        {
            if (rootDirectory[rootDirectory.Length - 1] != Path.DirectorySeparatorChar)
                rootDirectory += Path.DirectorySeparatorChar;
            _DirectoryInfoRoot = new DirectoryInfo(rootDirectory.TrimEnd('\\', '/'));
            _NCharactersEachLevel = nCharactersEachLevel;
            if (extension.Length < 1 || extension[0] != '.')
                throw new ArgumentException($"Invalid extension \"{extension}\"");
            _Extension = extension;
            _IdentifierBasedFileLock = identifierLock;
        }
        public bool Has(TIdentifier identifier)
        {
            string identifierString = PathHelper.EscapeIdentifier(identifier.ToString());
            return _IdentifierBasedFileLock.LockForReads(identifier, () =>
            {
                GetDirectoryPathAndFilePathFromIdentifier(identifierString,
                    out string directoryPath, out string filePath);
                bool fileExists = File.Exists(filePath);
                return fileExists;
            });
        }
        public void ReadCallbackDeleteWithinLock(TIdentifier identifier,
            Action<string> callback)
        {
            string stringIdentifier = PathHelper.EscapeIdentifier(identifier.ToString());
            _IdentifierBasedFileLock.LockForWrite(identifier, () =>
            {

                GetDirectoryPathAndFilePathFromIdentifier(stringIdentifier,
                    out string directoryPath, out string filePath);
                string content = null;
                try
                {
                    content = _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                    .LockForReads(filePath, () => File.ReadAllText(filePath));
                }
                catch (IOException ex) { }
                callback(content);
                _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                .LockForWrite(filePath, () => File.Delete(filePath));
            });

        }
        public void ReadCallbackWriteWithinLock(TIdentifier identifier, Func<string, string> callback)
        {

            string stringIdentifier = PathHelper.EscapeIdentifier(identifier.ToString());
            _IdentifierBasedFileLock.LockForWrite(identifier, () =>
            {

                GetDirectoryPathAndFilePathFromIdentifier(stringIdentifier,
                    out string directoryPath, out string filePath);
                string content = null;
                try
                {
                    content = _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                    .LockForReads(filePath, () => File.ReadAllText(filePath));
                }
                catch (IOException ex) { }
                callback(content);
                _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                    .LockForWrite(filePath, () =>
                    {
                        Directory.CreateDirectory(directoryPath);
                        File.WriteAllText(filePath, content);
                    });
            });
        }
        public void Write(TIdentifier identifier, string content)
        {
            string identifierString = PathHelper.EscapeIdentifier(identifier.ToString());
            GetDirectoryPathAndFilePathFromIdentifier(identifierString, out string directoryPath, out string filePath);
            _IdentifierBasedFileLock.LockForWrite(identifier, () =>
            {
                _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                    .LockForWrite(filePath, () =>
                    {
                        Directory.CreateDirectory(directoryPath);
                        File.WriteAllText(filePath, content);
                    });
            });
        }
        /// <summary>
        /// Will still lock the actual file but no injected lock
        /// </summary>
        /// <returns></returns>
        public string ReadWithoutLock(TIdentifier identifier)
        {
            string identifierString = PathHelper.EscapeIdentifier(identifier.ToString());
            GetDirectoryPathAndFilePathFromIdentifier(identifierString, out string ignore, out string filePath);
            try
            {
                return _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                .LockForReads(filePath, () => File.ReadAllText(filePath));
            }
            catch (IOException ex)
            {
                return null;
            }
        }
        public string Read(TIdentifier identifier)
        {
            string identifierString = PathHelper.EscapeIdentifier(identifier.ToString());
            GetDirectoryPathAndFilePathFromIdentifier(identifierString, out string ignore, out string filePath);
            return _IdentifierBasedFileLock.LockForReads(identifier, () =>
            {
                try
                {
                    return _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                    .LockForReads(filePath, () => File.ReadAllText(filePath));
                }
                catch (IOException ex)
                {
                    return null;
                }
            });
        }
        public void Delete(TIdentifier identifier)
        {
            string identifierString = PathHelper.EscapeIdentifier(identifier.ToString());
            GetDirectoryPathAndFilePathFromIdentifier(identifierString, out string directoryPath, out string filePath);
            _IdentifierBasedFileLock.LockForWrite(identifier, () =>
            {
                try
                {
                    _InternalFilePathIdentifierLock_NO_EXTERNAL_CALLBACKS
                        .LockForWrite(filePath, () =>
                        {
                            File.Delete(filePath);
                            DirectoryHelper.DeleteEmptyDirectoriesRecursively(_DirectoryInfoRoot, directoryPath);
                        });
                }
                catch { }
            });
        }
        private void GetDirectoryPathAndFilePathFromIdentifier(string identifier, out string directoryPath, out string filePath)
        {
            string identifierString = PathHelper.EscapeIdentifier(identifier.ToString());
            PathHelper.GetDirectoryPathAndFilePathFromIdentifier(identifierString, out directoryPath, out filePath, _DirectoryInfoRoot, _NCharactersEachLevel, _Extension);
        }
        public void IterateEntries(Action<DelegateNextEntry<string>> callback)
        {
            string rootDirectory = _DirectoryInfoRoot.FullName + Path.DirectorySeparatorChar;
            DelegateNextEntry<string> nextFile = DirectoryHelper.GetFilesRecursivelyAsynchronously(new DirectoryInfo(rootDirectory));
            callback(nextFile);
        }
    }
}