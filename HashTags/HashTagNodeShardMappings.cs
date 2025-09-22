using Core.Exceptions;
using Database;
using DependencyManagement;
using GlobalConstants;
namespace HashTags
{
    internal class HashTagNodeShardMappings
    {

        private static HashTagNodeShardMappings? _Instance;
        public static HashTagNodeShardMappings Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(HashTagNodeShardMappings));
            _Instance = new HashTagNodeShardMappings();
            return _Instance;
        }
        public static HashTagNodeShardMappings Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(HashTagNodeShardMappings));
                return _Instance;
            }
        }
        private int _MyNodeId;
        private string _DirectoryPathWithSeperatorOnEnd;
        public LocalSQLite[] LocalShards { get;}
        private ActivatedNodeShardMapping _Root;
        private HashTagNodeShardMappings()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _DirectoryPathWithSeperatorOnEnd = DependencyManager.GetString(DependencyNames.HashTagsDatabaseDiretory);
            if (_DirectoryPathWithSeperatorOnEnd.Last() != Path.DirectorySeparatorChar)
                _DirectoryPathWithSeperatorOnEnd += Path.DirectorySeparatorChar;
            List<LocalSQLite> localShards = new List<LocalSQLite>();
            _Root = ActivateRecursively(
#if DEBUG
                GlobalConstants.Nodes.HASH_TAGS_NODE_SHARD_MAPPINGS_DEBUG
#else
                GlobalConstants.Nodes.HASH_TAGS_NODE_SHARD_MAPPINGS
#endif
                , new List<string>(), localShards, isRoot: true);
            LocalShards = localShards.ToArray();
        }
        #region Methods
        #region Public
        public int GetNodeId(string tag)
        {
            if (tag == null || tag.Length <= 0)
                throw new ArgumentException(nameof(tag));
            return _Root.GetNode(tag, 0).NodeId;
        }
        public LocalSQLite GetShard(string tag)
        {
            if (tag == null || tag.Length <= 0)
                throw new ArgumentException(nameof(tag));
            LocalSQLite? localSQLite = _Root.GetNode(tag, 0).Shard;
            if (localSQLite == null)
                throw new NullReferenceException($"{nameof(localSQLite)} was null for tag \"{tag}\". This shouldnt happen");
            return localSQLite;
        }
        #endregion Public
        #region Private
        private ActivatedNodeShardMapping ActivateRecursively(HashTagsNodeShardMapping mapping, List<string> segments, List<LocalSQLite> localShards, bool isRoot = false)
        {
            Dictionary<char, ActivatedNodeShardMapping>? mapChildCharsToChild = null;
            if (isRoot)
            {
                if (mapping.Chars != null)
                    throw new ArgumentException($"{nameof(mapping.Chars)} must be null for root");
            }
            Func<string> getPath = () => $"\"{string.Join(Path.DirectorySeparatorChar, segments)}\"";
            if (mapping.Children != null)
            {
                foreach (HashTagsNodeShardMapping child in mapping.Children)
                {

                    char[] childChars = child.Chars;
                    if (childChars == null || childChars.Length < 0)
                        throw new InvalidDataException($"No characters were provided for a non root node at path {getPath()}");
                    string childCharsJoined = new string(childChars);
                    CheckForDuplicateChars(childCharsJoined, getPath);
                    segments.Add(childCharsJoined);
                    ActivatedNodeShardMapping activatedChild = ActivateRecursively(child, segments, localShards);
                    segments.RemoveAt(segments.Count - 1);//replace with linked list
                    MapChildCharsToChild(ref mapChildCharsToChild, childChars, activatedChild, getPath);
                }
            }
            LocalSQLite? localShard;
            if (mapping.NodeId == _MyNodeId) {
                localShard = CreateShardForLocalMapping(segmentsJoined: isRoot ? "_" : string.Join(Path.DirectorySeparatorChar, segments));
                localShards.Add(localShard);
            }
            else
            {
                localShard = null;
            }
            return new ActivatedNodeShardMapping(localShard, mapChildCharsToChild, mapping.NodeId);
        }
        private void MapChildCharsToChild(ref Dictionary<char, ActivatedNodeShardMapping>? mapChildCharsToChild,
            char[] childChars, ActivatedNodeShardMapping activatedChild, Func<string> getPath)
        {

            foreach (char childC in childChars)
            {
                if (mapChildCharsToChild == null)
                {
                    mapChildCharsToChild = new Dictionary<char, ActivatedNodeShardMapping>
                        {
                            { childC, activatedChild }
                        };
                    continue;
                }
                if (mapChildCharsToChild.ContainsKey(childC))
                {
                    throw new Exception($"Duplicate character '{childC}' on more than one child at parent path \"{getPath}\"");
                }
                mapChildCharsToChild.Add(childC, activatedChild);
            }
        }
        private void CheckForDuplicateChars(string charsJoined, Func<string> getPath)
        {
            char[] duplicatedCharacters = charsJoined.GroupBy(c => c)
            .Where(g => g.Count() > 1)
            .Select(g => g.First())
            .ToArray();
            if (duplicatedCharacters.Any())
            {
                throw new InvalidDataException($"Duplicated characters [{string.Join(',', duplicatedCharacters)}] at path {getPath()}");
            }
        }
        private LocalSQLite CreateShardForLocalMapping(string segmentsJoined)
        {
            string filePath = $"{_DirectoryPathWithSeperatorOnEnd} {segmentsJoined}.sqlite";
            LocalSQLite shard =  new LocalSQLite(filePath, true, GlobalConstants.Sizes.MAX_N_CONNECTIONS_HASH_TAGS_SHARD);
            return shard;
        }
        #endregion Private
        #endregion Methods
    }
}