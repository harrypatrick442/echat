using Core.Assets;
using Core.Exceptions;
using KeyValuePairDatabases;
using NodeAssignedIdRanges;
using Chat.Messages.Client;
using Chat.Interfaces;
using Database;
using Core.Pool;
using Microsoft.Data.Sqlite;
using Logging;
namespace Core.DAL
{
    public class DalNotificationsSQLite
    {
        //in memory cache with indices built from dictionaries ?

        //notificationType  Seperate table?
        //myUserId,
        //specificIdentifier (conversationId), otherUserId, time, spaceName, substring
        //pm notification: only one per specificIdentifier. 

        private const int MAX_N_CONNECTIONS = 4;
        private const string 
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblMessages (id INTEGER UNIQUE NOT NULL, version SMALL NOT NULL," +
                " sentAt INTEGER NOT NULL, userId INTEGER NOT NULL, content TEXT NOT NULL);"
                + " CREATE INDEX IF NOT EXISTS indexId ON tblMessages (id);"
                + " CREATE INDEX IF NOT EXISTS indexSentAt ON tblMessages (sentAt);"
                + " CREATE INDEX IF NOT EXISTS indexUserId ON tblMessages (userId);"
                + "CREATE TABLE IF NOT EXISTS tblMentions (userId INTEGER NOT NULL, messageId INTEGER NOT NULL);"
                + " CREATE INDEX IF NOT EXISTS indexUserId ON tblMentions (userId);"
                + " CREATE INDEX IF NOT EXISTS indexMessageId ON tblMentions (messageId);",
            READ_FROM_END_COMMAND = "SELECT id, version," +
                "sentAt, userId, content FROM tblMessages ORDER BY sentAt DESC LIMIT @nMessages;",
            READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND = "SELECT id, version," +
                "sentAt, userId, content FROM tblMessages WHERE id < @idToExclusive ORDER BY sentAt DESC LIMIT @nMessages;",
            INSERT_COMMAND = "INSERT INTO tblMessages (id, version,sentAt,userId,content)"+
            " VALUES(@id, @version,@sentAt,@userId,@content);";
        private NodesIdRangesForIdTypeManager _NodeIdRangesForMessagesManager;
        private static DalMessagesSQLite _Instance;
        public static DalMessagesSQLite Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMessagesSQLite));
                return _Instance;
            }
        }
        public static DalMessagesSQLite Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMessagesSQLite));
            _Instance = new DalMessagesSQLite();
            return _Instance;
        }
        private KeyValuePairInMemoryDatabase<long, LocalSQLite> _ConversationIdToLocalSQLiteKeyValuePairDatabase;
        private IIdentifierLock<long> _IdentifierLockConversationId;
        private DalNotificationsSQLite()
        {
            Directory.CreateDirectory(Paths.MessagesDatabaseDirectory);
            _IdentifierLockConversationId= new IdentifierLock<long>();
            _ConversationIdToLocalSQLiteKeyValuePairDatabase = 
                new KeyValuePairInMemoryDatabase<long, LocalSQLite>(
                new OverflowParameters<long, LocalSQLite>(false,
                _IdentifierLockConversationId, OverflowLocalSQLite),
                _IdentifierLockConversationId);
        }
        private void OverflowLocalSQLite(long conversationId, LocalSQLite localSQLite) {
            localSQLite.Dispose();
        }
        public void Append(long conversationId, ClientMessage message)
        {
            UsingConnection(conversationId, (connection) => {
                using (SqliteCommand command = new SqliteCommand(
                    INSERT_COMMAND, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@id", message.Id));
                    command.Parameters.Add(new SqliteParameter("@version", message.Version));
                    command.Parameters.Add(new SqliteParameter("@sentAt", message.SentAt));
                    command.Parameters.Add(new SqliteParameter("@userId", message.UserId));
                    command.Parameters.Add(new SqliteParameter("@content", message.Content));
                    command.ExecuteNonQuery();
                }
            });
        }
        public ClientMessage[] ReadFromEnd(long conversationId, int nMessages)
        {
            List<ClientMessage> messages = new List<ClientMessage>();
            UsingConnection(conversationId, (connection) => {

                using (SqliteCommand command = new SqliteCommand(
                    READ_FROM_END_COMMAND,
                    connection))
                {
                    command.Parameters.Add(new SqliteParameter("@nMessages", nMessages));
                    ReadClientMessages(command, messages);
                }
            });
            return ((IEnumerable<ClientMessage>)messages).Reverse().ToArray();
        }
        public ClientMessage[] ContinueReadBackwards(long conversationId, int nMessages,
            long idToExclusive)
        {
            List<ClientMessage> messages = new List<ClientMessage>();
            UsingConnection(conversationId, (connection) => {

                using (SqliteCommand command = new SqliteCommand(
                    READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND,
                    connection))
                {
                    command.Parameters.Add(new SqliteParameter("@idToExclusive", idToExclusive));
                    command.Parameters.Add(new SqliteParameter("@nMessages", nMessages));
                    ReadClientMessages(command, messages);
                }
            });
            return ((IEnumerable<ClientMessage>)messages).Reverse().ToArray();
        }
        private void UsingConnection(long conversationId, Action<SqliteConnection> callback) {
            ObjectPoolHandle<SqliteConnection> handle = null;
            try
            {
                handle =
                _ConversationIdToLocalSQLiteKeyValuePairDatabase.ReadWithinLock(
                    conversationId, (localSqlite) => localSqlite?.TakeConnectionDangerous());
                if (handle != null)
                {
                    callback(handle.Object);
                    return;
                }
                _ConversationIdToLocalSQLiteKeyValuePairDatabase.ReadCallbackWriteWithinLock(
                    conversationId, (localSqlite) =>
                {
                    if (localSqlite == null)
                    {
                        localSqlite = new LocalSQLite(
                            Path.Combine(Paths.MessagesDatabaseDirectory, $"{conversationId}.sqlite"),
                            true, maxNConnections:MAX_N_CONNECTIONS);
                        localSqlite.UsingConnection(PrepareConversationDatabase);
                    }
                    handle = localSqlite.TakeConnectionDangerous();
                    return localSqlite;
                });
                callback(handle.Object);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            finally {
                handle?.Dispose();
            }
        }
        private void ReadClientMessages(SqliteCommand command, List<ClientMessage> messages) {

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    int version = reader.GetInt32(1);
                    long sentAt = reader.GetInt64(2);
                    long userId = reader.GetInt64(3);
                    string content = reader.GetString(4);
                    messages.Add(new ClientMessage(id, version, sentAt, userId, content));
                }
            }
        }
        private void PrepareConversationDatabase(SqliteConnection connection)
        {
            using (SqliteCommand command = new SqliteCommand(CREATE_COMMAND,
                connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}