using Database;
using Microsoft.Data.Sqlite;
using System.Data;
using Core.Timing;
using System.Text;
using Core.Exceptions;
using HashTags.Messages;
using DependencyManagement;
using HashTags.Enums;
using Initialization.Exceptions;
namespace HashTags.DAL
{
    internal class DalHashTags
    {
        private static DalHashTags? _Instance;
        public static DalHashTags Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalHashTags));
                return _Instance;
            }
        }
        public static DalHashTags Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalHashTags));
            _Instance = new DalHashTags();
            return _Instance;
        }
        private const int MAX_N_CONNECTIONS = 4;
        private const string
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblHashTags(tag TEXT NOT NULL, scopeType SMALL NOT NULL, " +
                "scopeId INTEGER NOT NULL, scopeId2 INTEGER, atTime INTEGER NOT NULL);" +
                "CREATE INDEX IF NOT EXISTS ix_tblHashTags_scopeType_tag ON tblHashTags(scopeType, tag);" +
                "CREATE INDEX IF NOT EXISTS ix_tblHashTags_scopeType_scopeId_scopeId2 ON tblHashTags(scopeType, scopeId, scopeId2);" +
                "CREATE INDEX IF NOT EXISTS ix_tblHashTags_scopeType_atTime ON tblHashTags(scopeType, atTime);",
            ADD_COMMAND = "INSERT INTO tblHashTags(tag,scopeType,scopeId,scopeId2,atTime) VALUES",
            DELETE_COMMAND_="DELETE FROM tblHashTags WHERE scopeType = @scopeType AND scopeId = @scopeId " +
            "AND scopeId2 IS @scopeId2 AND tag IN (",
            SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_COMMAND = "SELECT scopeId, scopeId2 FROM tblHashTags " +
            "WHERE scopeType = @scopeType AND tag = @tag ORDER BY atTime DESC LIMIT @maxNEntries;",
            SEARCH_EXACT_MATCHES_WITHOUT_SCOPE_TYPE_COMMAND = "SELECT scopeId, scopeId2 FROM tblHashTags " +
            "WHERE tag = @tag ORDER BY atTime DESC LIMIT @maxNEntries;",
            /*SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_UNIQUE_ON_SCOPEID_COMMAND = "SELECT scopeId, scopeId2 FROM tblHashTags " +
            "WHERE scopeType = @scopeType AND tag = @tag ORDER BY atTime DESC LIMIT @maxNEntries;",
            SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_UNIQUE_ON_SCOPEID2_COMMAND = "SELECT scopeId, scopeId2 FROM tblHashTags " +
            "WHERE scopeType = @scopeType AND tag = @tag ORDER BY atTime DESC LIMIT @maxNEntries;",
            SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_UNIQUE_ON_SCOPEIDS_COMMAND = "SELECT scopeId, scopeId2 FROM tblHashTags " +
            "WHERE scopeType = @scopeType AND tag = @tag ORDER BY atTime DESC LIMIT @maxNEntries;",*/
            SEARCH_PARTIAL_MATCHES_WITH_SCOPE_TYPE_COMMAND = "SELECT tag, scopeId, scopeId2 FROM tblHashTags " +
            "WHERE scopeType = @scopeType AND tag != @tag AND tag LIKE @tagForLike ESCAPE '\\' ORDER BY atTime DESC LIMIT @maxNEntries;",
            SEARCH_PARTIAL_MATCHES_WITHOUT_SCOPE_TYPE_COMMAND = "SELECT tag, scopeId, scopeId2 FROM tblHashTags " +
            "WHERE tag != @tag AND tag LIKE @tagForLike ESCAPE '\\' ORDER BY atTime DESC LIMIT @maxNEntries;",
            SEARCH_TO_PREDICT_TAG_WITH_SCOPE_TYPE_COMMAND = "SELECT DISTINCT tag FROM tblHashTags " +
            "WHERE scopeType = @scopeType AND tag LIKE @tag ESCAPE '\\' LIMIT @maxNEntries;",
            SEARCH_TO_PREDICT_TAG_WITHOUT_SCOPE_TYPE_COMMAND = "SELECT DISTINCT tag FROM tblHashTags " +
            "WHERE tag LIKE @tag ESCAPE '\\' LIMIT @maxNEntries;";
        private DalHashTags()
        {
            string directoryPath = DependencyManager.GetString(DependencyNames.HashTagsDatabaseDiretory);
            Directory.CreateDirectory(directoryPath);
            foreach (LocalSQLite shard in HashTagNodeShardMappings.Instance.LocalShards)
            {
                PrepareHashTagsDatabase(shard);
            }
        }
        public void AddTags(string[] tags, HashTagScopeTypes scopeType, long scopeId, long? scopeId2)
        {
            long millisecondsNow = TimeHelper.MillisecondsNow;
            ShardTagsPair[] shardTagsPairs = GetShardTagsPairs(tags);
            foreach (ShardTagsPair shardTagsPair in shardTagsPairs) {
                shardTagsPair.Shard.UsingConnectionForWrite((connection) => {
                    using (var transaction = connection.BeginTransaction())
                    {
                        StringBuilder sbCommand = new StringBuilder();
                        sbCommand.Append(ADD_COMMAND);
                        bool first = true;
                        string atTime = TimeHelper.MillisecondsNow.ToString();
                        string scopeId2String = scopeId2 == null ? "NULL" : ((long)scopeId2).ToString();
                        //No need to use parameters here. Tag is highly restricted to only certain characters and those characters are safe.
                        foreach (string tag in tags)
                        {
                            if (first) first = false;
                            else
                                sbCommand.Append(",");
                            sbCommand.Append("('");
                            sbCommand.Append(tag);
                            sbCommand.Append("',");
                            sbCommand.Append((int)scopeType);
                            sbCommand.Append(",");
                            sbCommand.Append(scopeId);
                            sbCommand.Append(",");
                            sbCommand.Append(scopeId2String);
                            sbCommand.Append(",");
                            sbCommand.Append(atTime);
                            sbCommand.Append(")");
                        }
                        sbCommand.Append(";");
                        using (SqliteCommand command = new SqliteCommand(
                            sbCommand.ToString(), connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                });
            }
        }
        public void Delete(HashTagScopeTypes scopeType, long scopeId, long? scopeId2, string[] tags)
        {
            ShardTagsPair[] shardTagsPairs = GetShardTagsPairs(tags);
            foreach (ShardTagsPair shardTagsPair in shardTagsPairs)
            {
                shardTagsPair.Shard.UsingConnectionForWrite((connection) => {
                    using (var transaction = connection.BeginTransaction())
                    {
                        StringBuilder sbCommand = new StringBuilder(DELETE_COMMAND_);
                        bool first = true;
                        foreach (string tag in shardTagsPair.Tags)
                        {
                            if (first) first = false;
                            else 
                                sbCommand.Append(',');
                            sbCommand.Append('\'');
                            sbCommand.Append(tag);
                            sbCommand.Append('\'');
                        }
                        sbCommand.Append(")");
                        using (SqliteCommand command = new SqliteCommand(
                            sbCommand.ToString(), connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@scopeType", (int)scopeType));
                            command.Parameters.Add(new SqliteParameter("@scopeId", scopeId));
                            command.Parameters.Add(new SqliteParameter("@scopeId2", scopeId2 == null ? DBNull.Value : (int)scopeId2));
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="scopeType"></param>
        /// <param name="allowPartialMatches">is always a partial match from start of word atm.
        /// This fits nicely with the sharding and the simplicity of tags for predicting what a user might be typing too</param>
        /// <param name="maxNEntries"></param>
        /// <param name="partialMatches"></param>
        /// <returns></returns>
        public ScopeIds[] Search(string tag, HashTagScopeTypes? scopeType,
            bool allowPartialMatches, int maxNEntries, out TagWithScopeIds[]? partialMatches/*, UniqueOn uniqueOn*/)
        {
            LocalSQLite shard = HashTagNodeShardMappings.Instance.GetShard(tag);
            TagWithScopeIds[]? partialMatchesInternal = null;
            ScopeIds[]? exactMatches = null;
            shard.UsingConnection((connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    if (scopeType != null)
                    {/*
                        string commandString = uniqueOn switch
                        {
                            UniqueOn.None => SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_COMMAND,
                            UniqueOn.ScopeId => SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_UNIQUE_ON_SCOPEID_COMMAND,
                            UniqueOn.ScopeId2 => SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_UNIQUE_ON_SCOPEID2_COMMAND,
                            UniqueOn.BothScopeIds => SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_UNIQUE_ON_SCOPEIDS_COMMAND
                        };*/
                        using (SqliteCommand command = new SqliteCommand(
                            SEARCH_EXACT_MATCHES_WITH_SCOPE_TYPE_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@tag", tag));
                            command.Parameters.Add(new SqliteParameter("@scopeType", scopeType));
                            command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries));
                            exactMatches = ReadExactMatchSearchResults(command).ToArray();
                        }
                    }
                    else
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            SEARCH_EXACT_MATCHES_WITHOUT_SCOPE_TYPE_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@tag", tag));
                            command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries));
                            exactMatches = ReadExactMatchSearchResults(command).ToArray();
                        }
                    }
                    if (!allowPartialMatches || exactMatches.Length >= maxNEntries)
                    {
                        return;
                    }
                    if (scopeType != null)
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            SEARCH_PARTIAL_MATCHES_WITH_SCOPE_TYPE_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@tagForLike", $"{tag}%"));
                            command.Parameters.Add(new SqliteParameter("@tag", tag));
                            command.Parameters.Add(new SqliteParameter("@scopeType", scopeType));
                            command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries - exactMatches.Length));
                            partialMatchesInternal = ReadPartialMatchSearchResults(command).ToArray();
                        }
                    }
                    else
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            SEARCH_PARTIAL_MATCHES_WITHOUT_SCOPE_TYPE_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@tagForLike", $"{tag}%"));
                            command.Parameters.Add(new SqliteParameter("@tag", tag));
                            command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries - exactMatches.Length));
                            partialMatchesInternal = ReadPartialMatchSearchResults(command).ToArray();
                        }
                    }
                }
            });
            partialMatches = partialMatchesInternal;
            return exactMatches!;
        }
        public string[] SearchToPredictTag(string tag, HashTagScopeTypes? scopeType, int maxNEntries)
        {
            string[] matches = null;
            LocalSQLite shard = HashTagNodeShardMappings.Instance.GetShard(tag);
            shard.UsingConnection((connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    if (scopeType != null)
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            SEARCH_TO_PREDICT_TAG_WITH_SCOPE_TYPE_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@tag", $"{tag}%"));
                            command.Parameters.Add(new SqliteParameter("@scopeType", scopeType));
                            command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries));
                            matches = ReadPredictTagSearchResults(command).ToArray();
                        }
                    }
                    else
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            SEARCH_TO_PREDICT_TAG_WITHOUT_SCOPE_TYPE_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@tag", $"{tag}%"));
                            command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries));
                            matches = ReadPredictTagSearchResults(command).ToArray();
                        }
                    }
                }
            });
            return matches;
        }
        private IEnumerable<string> ReadPredictTagSearchResults(SqliteCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return reader.GetString(0);
                }
            }
        }
        private IEnumerable<ScopeIds> ReadExactMatchSearchResults(SqliteCommand command) {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new ScopeIds(reader.GetInt64(0), reader.IsDBNull(1) ? null : reader.GetInt64(1));
                }
            }
        }
        private IEnumerable<TagWithScopeIds> ReadPartialMatchSearchResults(SqliteCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new TagWithScopeIds(reader.GetString(0), reader.GetInt64(1), reader.IsDBNull(2) ? null : reader.GetInt64(2));
                }
            }
        }
        public void PrepareHashTagsDatabase(LocalSQLite shard)
        {
            shard.UsingConnectionForWrite(connection =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(CREATE_COMMAND,
                        connection, transaction))
                    {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            });
        }
        private ShardTagsPair[] GetShardTagsPairs(string[] tags)
        {
            return tags.Select(tag => new { tag, shard = HashTagNodeShardMappings.Instance.GetShard(tag) })
                .GroupBy(o => o.shard)
                .Select(g => new ShardTagsPair(
                    g.First().shard,
                    g.Select(g => g.tag).ToArray()
                 )).ToArray();
        }
    }
}