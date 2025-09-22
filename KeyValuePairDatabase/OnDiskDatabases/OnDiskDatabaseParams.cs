
using Core.FileSystem;
using Core.Delegates;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.OnDiskDatabases;

namespace KeyValuePairDatabases
{
    public class OnDiskDatabaseParams
    {
        public string RootDirectory { get; set; }
        public string FilePath { get; set; }
        public int NCharactersEachLevel { get; set; }
        public string Extension { get; set; }
        public int StringKeyLength { get; set; }
        public Type[] KnownTypes { get; set; }
    }
}