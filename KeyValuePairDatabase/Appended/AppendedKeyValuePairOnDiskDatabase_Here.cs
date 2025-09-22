using Core.FileSystem;
using Core.Timing;

namespace KeyValuePairDatabases.Appended
{
    public partial class AppendedKeyValuePairOnDiskDatabase<TEntry>
    {
        private void _Append_Here(long identifier, string entry, out long indexToContinueFromToGoBackFromMessage)
        {
            long indexToContinueFromToGoBackFromMessage_Internal = 0;
            _IdentifierLock.LockForWrite(identifier, () => {
                AppendedMetadata_File currentFile;
                bool createdNextFile = true;
                AppendedMetadata appendedLinesMetadata = _MapFileIdToAppendedLineMetadataKeyValuePairDatabase.Get(identifier);
                if (appendedLinesMetadata == null)
                {
                    appendedLinesMetadata = new AppendedMetadata();
                    currentFile = appendedLinesMetadata.CreateNextFile();
                }
                else
                {
                    currentFile = appendedLinesMetadata.CurrentFile;
                    if (currentFile.Length > MAX_FILE_LENGTH_BYTES)
                        currentFile = appendedLinesMetadata.CreateNextFile();
                    else createdNextFile = false;
                }
                string filePath = _GetFilePath(identifier, currentFile.NFile, out string ignore);
                if (createdNextFile)
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                indexToContinueFromToGoBackFromMessage_Internal = currentFile.EndIndexExclusive;
                currentFile.Length += NullDelimitedAppendedJsonStringsFileHelper.Append(filePath, entry);
                _MapFileIdToAppendedLineMetadataKeyValuePairDatabase.Set(identifier, appendedLinesMetadata);
            });
            indexToContinueFromToGoBackFromMessage = indexToContinueFromToGoBackFromMessage_Internal;
        }
        private List<string> _Read_Here(long identifier,
            long? indexToReadFromBackwardsExclusive,
            int nEntries, out long toIndexFromBeginningExclusive)
        {
            long toIndexFromBeginningExclusive_Internal = -1;
            List<string> entries = new List<string>();
            _IdentifierLock.LockForReads(identifier, () => {
                AppendedMetadata appendedLinesMetadata = _MapFileIdToAppendedLineMetadataKeyValuePairDatabase.Get(identifier);
                if (appendedLinesMetadata == null)
                    return;
                AppendedMetadata_File currentFile;
                int fileIndex = 0;
                if (indexToReadFromBackwardsExclusive == null)
                {
                    currentFile = appendedLinesMetadata.CurrentFile;
                    if (currentFile == null)
                        return;
                    indexToReadFromBackwardsExclusive = currentFile.EndIndexExclusive;
                }
                else
                {
                    currentFile = appendedLinesMetadata.GetFileContainingStartIndex(
                        (long)indexToReadFromBackwardsExclusive, out fileIndex);
                    long endIndexExclusive = currentFile.EndIndexExclusive;
                    if (indexToReadFromBackwardsExclusive > endIndexExclusive)
                        indexToReadFromBackwardsExclusive = endIndexExclusive;
                }
                while (currentFile != null)
                {
                    string filePath = _GetFilePath(identifier, currentFile.NFile, out string directoryPath);
                    long? indexToReadFromBackwardsExclusive_WithinThisFile = indexToReadFromBackwardsExclusive == null
                            ? null
                            : ((long)indexToReadFromBackwardsExclusive)
                                - currentFile.StartIndexInclusive;
                    NullDelimitedAppendedJsonStringsFileHelper.
                        Read(filePath, entries,
                        nEntries: nEntries - entries.Count,
                        out long nextStartIndexFromBeginningExclusive_WithinThisFile,
                        indexToReadFromBackwardsExclusive_WithinThisFile);
                    indexToReadFromBackwardsExclusive = null;
                    toIndexFromBeginningExclusive_Internal = currentFile.StartIndexInclusive + nextStartIndexFromBeginningExclusive_WithinThisFile;
                    if (entries.Count >= nEntries)
                    {
                        return;
                    }
                    currentFile = appendedLinesMetadata.GetFileAtIndex(++fileIndex);
                }
            });
            toIndexFromBeginningExclusive = toIndexFromBeginningExclusive_Internal;
            return entries;
        }
        private string _GetFilePath(long identifier, int currentNFile, out string directoryPath)
        {
            string identifierString = $"{identifier}_{currentNFile}";
            PathHelper.GetDirectoryPathAndFilePathFromIdentifier(identifierString, out directoryPath, out string filePath, _DirectoryInfoRoot, _NCharactersEachLevel, ".bin");
            return filePath;
        }
    }
}