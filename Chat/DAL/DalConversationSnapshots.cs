using Microsoft.Data.Sqlite;
using SQLite;
using System.Data;
using Chat;
using Logging;
using Core.Exceptions;
using DependencyManagement;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using Core.Enums;
namespace Core.DAL
{
    public class DalConversationSnapshots
    {
        private const int MAX_N_CONNECTIONS = 4;
        private const string
            SELECTS = "messageId, version, sentAt, messageUserId,  content, conversationId, " +
            "conversationType, userIdsInConversation, seen ",
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblConversationSnapshots(" +
                    "messageId INTEGER NOT NULL," +
                    "myUserId INTEGER NOT NULL," +
                    "conversationId INTEGER NOT NULL," +
                    "conversationType SMALL NOT NULL," +
                    "sentAt INTEGER NOT NULL," +
                    "messageUserId INTEGER NOT NULL," +
                    "version SMALL NOT NULL," +
                    "userIdsInConversation TEXT NOT NULL," +
                    "content TEXT," +
                    "seen BOOLEAN NOT NULL," +
                    "PRIMARY KEY (myUserId, conversationId));" +
                "CREATE INDEX IF NOT EXISTS ix_tblConversationSnapshots_myUserId_conversationId_sentAt ON tblConversationSnapshots(myUserId, conversationId, sentAt);",
           READ_FROM_WHERE = "FROM tblConversationSnapshots a WHERE a.myUserId= @myUserId ",
        READ_FROM_END_COMMAND = "SELECT " + SELECTS +
            READ_FROM_WHERE + "ORDER BY a.messageId DESC LIMIT @nMessages;",
        READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND = "SELECT " + SELECTS +
            READ_FROM_WHERE + "AND a.messageId < @idToExclusive ORDER BY a.messageId DESC LIMIT @nMessages;",
        READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND = "SELECT " + SELECTS +
            READ_FROM_WHERE + "AND a.messageId >= @idFromInclusive ORDER BY a.messageId LIMIT @nMessages;",
        READ_EACH_SIDE_OF_ID_COMMAND = "SELECT * FROM (SELECT " + SELECTS +
            READ_FROM_WHERE + "AND a.messageId < @id ORDER BY a.messageId DESC LIMIT @nMessagesEitherSide) " +
            "UNION SELECT * FROM (SELECT " + SELECTS +
            READ_FROM_WHERE + "AND a.messageId >= @id ORDER BY a.messageId LIMIT @nMessagesEitherSide) ORDER BY id;",
        READ_BETWEEN_IDS_COMMAND = "SELECT " + SELECTS +
            READ_FROM_WHERE + "AND a.messageId < @idToExclusive AND a.messageId>=@idFromInclusive ORDER BY a.messageId LIMIT @nMessages;",

            INSERT_OR_REPLACE_COMMAND = "INSERT OR REPLACE INTO tblConversationSnapshots(" +
                "messageId, myUserId, conversationId, conversationType, " +
                "sentAt, messageUserId, version, userIdsInConversation, content, seen) " +
                "VALUES(@messageId, @myUserId, @conversationId, @conversationType, " +
                "@sentAt, @messageUserId, @version, @userIdsInConversation, @content, @seen);",
            SET_SEEN_COMMAND = "update tblConversationSnapshots " +
            "set seen = TRUE where  myUserId = @myUserId AND conversationId = @conversationId AND messageId = @messageId;",
            DELETE_ALL_FOR_USER_COMMAND = "DELETE FROM tblConversationSnapshots WHERE userId = @userId";
        private static DalConversationSnapshots _Instance;
        public static DalConversationSnapshots Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalConversationSnapshots));
                return _Instance;
            }
        }
        public static DalConversationSnapshots Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalConversationSnapshots));
            _Instance = new DalConversationSnapshots();
            return _Instance;
        }
        private LocalSQLiteShards _Shards;
        protected DalConversationSnapshots()
        {
            string directory = DependencyManager.GetString(DependencyNames.UserIdToConversationSnapshotsDatabaseDirectory);
            Directory.CreateDirectory(directory);
            _Shards = new LocalSQLiteShards(
                directory,
                true, 
                ChatConstants.SHARD_SIZE_CONVERSATION_SNAPSHOTS,
                PrepareConversationDatabase,
                maxNConnections: MAX_N_CONNECTIONS
            );
        }
        public void SetSeen(long myUserId, long conversationId, long messageId) {

            _Shards.UsingConnectionForWrite(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        SET_SEEN_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", messageId));
                        command.Parameters.Add(new SqliteParameter("@conversationId", conversationId));
                        command.Parameters.Add(new SqliteParameter("@myUserId", myUserId));
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            });
        }
        public void InsertOrReplace(long myUserId, long conversationId,
            ConversationSnapshot conversationSnapshot)
        {
            _Shards.UsingConnectionForWrite(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        INSERT_OR_REPLACE_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", conversationSnapshot.Id));
                        command.Parameters.Add(new SqliteParameter("@myUserId", myUserId));
                        command.Parameters.Add(new SqliteParameter("@version", conversationSnapshot.Version));
                        command.Parameters.Add(new SqliteParameter("@sentAt", conversationSnapshot.SentAt));
                        command.Parameters.Add(new SqliteParameter("@messageUserId", conversationSnapshot.UserId));
                        command.Parameters.Add(new SqliteParameter("@content", conversationSnapshot.Content == null ? DBNull.Value : conversationSnapshot.Content));
                        command.Parameters.Add(new SqliteParameter("@conversationId", conversationSnapshot.ConversationId));
                        command.Parameters.Add(new SqliteParameter("@conversationType", conversationSnapshot.ConversationType));
                        command.Parameters.Add(new SqliteParameter("@userIdsInConversation", string.Join(',', conversationSnapshot.UserIdsInConversation)));
                        command.Parameters.Add(new SqliteParameter("@seen", conversationSnapshot.UserId== myUserId? true : false));
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            });
        }
        public ConversationSnapshot[] ReadRange(long myUserId, int? nMessages,
            long? idFromInclusive, long? idToExclusive/*, out MessageReaction[]? reactions,
            out MessageUserMultimediaItem[]? messageUserMultimediaItemss*/)
        {
            RangeType rangeType =
                idFromInclusive == null
                ? (
                    idToExclusive == null
                    ? RangeType.Default
                    : RangeType.ToToExclusive
                )
                : (
                    idToExclusive == null
                    ? RangeType.FromFromInclusive
                    : (
                        idToExclusive == idFromInclusive
                        ? RangeType.EitherSideOf
                        : RangeType.Between
                      )
                  );
            List<ConversationSnapshot> conversationSnapshots = new List<ConversationSnapshot>();
            string commandString;
            bool reverse = false;
            switch (rangeType)
            {
                case RangeType.ToToExclusive:
                    commandString = READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND;
                    reverse = true;
                    break;
                case RangeType.FromFromInclusive:
                    commandString = READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND;
                    break;
                case RangeType.Between:
                    commandString = READ_BETWEEN_IDS_COMMAND;
                    break;
                case RangeType.EitherSideOf:
                    commandString = READ_EACH_SIDE_OF_ID_COMMAND;
                    break;
                default:
                case RangeType.Default:
                    commandString = READ_FROM_END_COMMAND;
                    reverse = true;
                    break;
            }
            //List<MessageReaction> reactionsList = null;
            //List<MessageUserMultimediaItem> messageUserMultimediaItemssList = null;
            _Shards.UsingConnection(myUserId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        commandString,
                        connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@myUserId", myUserId));
                        if (rangeType.Equals(RangeType.EitherSideOf))
                        {
                            command.Parameters.Add(new SqliteParameter("@id", idToExclusive));
                            command.Parameters.Add(new SqliteParameter("@nMessagesEitherSide", nMessages / 2));
                        }
                        else
                        {

                            command.Parameters.Add(new SqliteParameter("@nMessages", nMessages));
                            if (!rangeType.Equals(RangeType.Default))
                            {
                                if (rangeType.Equals(RangeType.Between) || rangeType.Equals(RangeType.FromFromInclusive))
                                {
                                    command.Parameters.Add(new SqliteParameter("@idFromInclusive", idFromInclusive));
                                }
                                if (rangeType.Equals(RangeType.Between) || rangeType.Equals(RangeType.ToToExclusive))
                                {
                                    command.Parameters.Add(new SqliteParameter("@idToExclusive", idToExclusive));
                                }
                            }
                        }
                        ReadConversationSnapshots(command, conversationSnapshots);
                        //command.CommandText = ConstructReadReactionsCommand(conversationSnapshots);
                        //ReadMessageReactions(command, ref reactionsList);
                        //command.CommandText = ConstructReadMessageUserMultimediaItemssCommand(messages);
                        //ReadMessageUserMultimediaItemss(command, ref messageUserMultimediaItemssList);
                    }
                }
            });
            //reactions = reactionsList?.ToArray();
            //messageUserMultimediaItemss = messageUserMultimediaItemssList?.ToArray();
            if (reverse)
                return ((IEnumerable<ConversationSnapshot>)conversationSnapshots).Reverse().ToArray();
            return conversationSnapshots.ToArray();
        }
        private void ReadConversationSnapshots(SqliteCommand command, List<ConversationSnapshot> conversationSnapshots)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    conversationSnapshots.Add(
                        new ConversationSnapshot(
                            reader.GetInt64(0),
                            reader.GetInt16(1),
                            reader.GetInt64(2),
                            reader.GetInt64(3),
                            reader.IsDBNull(4)?null:reader.GetString(4),
                            reader.GetInt64(5),
                            (ConversationType)reader.GetInt16(6),
                            reader.GetString(7).Split(',')
                                .Select(long.Parse).ToArray(),
                            reader.GetBoolean(8)
                        ));
                }
            }
        }
        public void DeleteAllForUser(long userId)
        {
            _Shards.UsingConnectionForWrite(userId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        DELETE_ALL_FOR_USER_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@userId", userId));
                        transaction.Commit();
                    }
                }
            });
        }
        public ConversationSnapshot[] ReadEntries(SqliteCommand command)
        {
            List<ConversationSnapshot> entries = null;
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read()) {
                    if (entries == null)
                        entries = new List<ConversationSnapshot>();
                    try
                    {
                        entries.Add(
                            new ConversationSnapshot(
                                reader.GetInt64(0),
                                reader.GetInt16(1),
                                reader.GetInt64(2),
                                reader.GetInt64(3),
                                reader.GetString(4),
                                reader.GetInt64(5),
                                (ConversationType)reader.GetInt16(6),
                                reader.GetString(7).Split(',')
                                    .Select(long.Parse).ToArray(),
                                reader.GetBoolean(8)
                            ));
                    }
                    catch (Exception ex) {
                        Logs.Default.Error(ex);
                    }
                }
                return entries?.ToArray();
            }
        }
        private void PrepareConversationDatabase(SqliteConnection connection)
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
        }
    }
}