using KeyValuePairDatabases;
using Chat.Interfaces;
using Database;
using Core.Pool;
using Microsoft.Data.Sqlite;
using Core.Enums;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using System.Text;
using MultimediaCore;
namespace Core.DAL
{
    public abstract class DalMessagesSQLiteSingleConversation : IDalMessages
    {
        private const int MAX_N_CONNECTIONS = 4;
        private const string
            SHORT_SELECTS = " id, version, sentAt, userId, content ",
            SELECTS = " a.id, a.version, a.sentAt, a.userId, a.content, a.replyTo, a.userId as replyUserId, b.content as replyContent, b.version as replyVersion ",
            CREATE_COMMAND =
                " CREATE TABLE IF NOT EXISTS tblMessages (id INTEGER UNIQUE NOT NULL, version SMALL NOT NULL," +
                    " sentAt INTEGER NOT NULL, userId INTEGER NOT NULL, content TEXT NOT NULL, replyTo INTEGER);" +
                " CREATE INDEX IF NOT EXISTS indexTblMessagesId ON tblMessages(id);" +
                " CREATE INDEX IF NOT EXISTS indexTblMessagesSentAt ON tblMessages(sentAt);" +
                " CREATE INDEX IF NOT EXISTS indexTblMessagesUserId ON tblMessages(userId);" +
                " CREATE TABLE IF NOT EXISTS tblReactionCodes (messageId INTEGER NOT NULL," +
                    " userId INTEGER NOT NULL, code INTEGER NOT NULL," +
                    " UNIQUE(messageId, userId, code));" +
                " CREATE INDEX IF NOT EXISTS indexTblReactionCodesMessageIdUserId ON tblReactionCodes(messageId,userId);" +
                " CREATE TABLE IF NOT EXISTS tblReactionMultimediaTokens (messageId INTEGER NOT NULL," +
                    " userId INTEGER NOT NULL, multimediaToken TEXT NOT NULL," +
                    " UNIQUE(messageId, userId, multimediaToken));" +
                " CREATE INDEX IF NOT EXISTS indexTblReactionMultimediaTokensMessageIdUserId " +
                    " ON tblReactionMultimediaTokens(messageId,userId);",
            REPLIES_LEFT_JOIN = "LEFT OUTER JOIN tblMessages b on a.replyTo = b.id ",
            READ_FROM_END_COMMAND = "SELECT" + SELECTS +
                "FROM tblMessages a " + REPLIES_LEFT_JOIN + "ORDER BY a.id DESC LIMIT @nMessages;",
            READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND = "SELECT" + SELECTS +
                "FROM tblMessages a " + REPLIES_LEFT_JOIN + "WHERE a.id < @idToExclusive ORDER BY a.id DESC LIMIT @nMessages;",
            READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND = "SELECT" + SELECTS +
                "FROM tblMessages a " + REPLIES_LEFT_JOIN + "WHERE a.id >= @idFromInclusive ORDER BY a.id LIMIT @nMessages;",
            READ_EACH_SIDE_OF_ID_COMMAND = "SELECT * FROM (SELECT" + SELECTS +
                "FROM tblMessages a " + REPLIES_LEFT_JOIN + "WHERE a.id < @id ORDER BY a.id DESC LIMIT @nMessagesEitherSide) " +
                "UNION SELECT * FROM (SELECT" + SELECTS +
                "FROM tblMessages WHERE a.id >= @id ORDER BY a.id LIMIT @nMessagesEitherSide) ORDER BY a.id;",
            READ_BETWEEN_IDS_COMMAND = "SELECT" + SELECTS +
                "FROM tblMessages a " + REPLIES_LEFT_JOIN + "WHERE a.id < @idToExclusive AND a.id>=@idFromInclusive ORDER BY a.id LIMIT @nMessages;",
            INSERT_COMMAND = "INSERT INTO tblMessages (id, version,sentAt,userId,content,replyTo)" +
            " VALUES(@id, @version,@sentAt,@userId,@content,@replyTo);",
            SELECT_INSERT_REPLY_COMMAND = "SELECT" + SHORT_SELECTS + "FROM tblMessages WHERE id=@replyTo;",
            DELETE_MESSAGES_COMMAND = "DELETE FROM tblMessages WHERE userId={0} AND id IN ({1}) RETURNING id",
            DELETE_ANY_MESSAGES_COMMAND = "DELETE FROM tblMessages WHERE id IN ({0}) RETURNING id",
            DELETE_REACTIONS_COMMAND_1 = "DELETE FROM tblReactionCodes WHERE messageId IN (",
            DELETE_REACTIONS_COMMAND_2 = "); DELETE FROM tblReactionMultimediaTokens WHERE messageId IN (",
            DELETE_REACTIONS_COMMAND_3=");",
            UPDATE_COMMAND = "UPDATE tblMessages " +
            "SET content=@content,version=@version WHERE id= @id AND userId=@userId",
            ADD_CODE_REACTION_COMMAND = "INSERT OR IGNORE INTO tblReactionCodes(messageId, userId, code) VALUES(@messageId, @userId, @code)",
            ADD_MULTIMEDIA_REACTION_COMMAND = "INSERT OR IGNORE INTO tblReactionMultimediaTokens(messageId, userId, multimediaToken) VALUES(@messageId, @userId, @multimediaToken)",
            READ_MESSAGE_REACTIONS_1 =
                "SELECT * FROM (SELECT messageId, userId, code, null as multimediaToken FROM tblReactionCodes" +
                " WHERE messageId IN (",
            READ_MESSAGE_REACTIONS_2 = ")) UNION SELECT * FROM (" +
            "SELECT messageId, userId, null as code, multimediaToken FROM tblReactionMultimediaTokens WHERE messageId IN (",
            READ_MESSAGE_REACTIONS_3 = "));",
            REMOVE_MULTIMEDIA_REACTION_COMMAND = "DELETE FROM tblReactionMultimediaTokens WHERE messageId=@messageId AND userId=@userId AND multimediaToken=@multimediaToken;",
            REMOVE_CODE_REACTION_COMMAND = "DELETE FROM tblReactionCodes WHERE messageId=@messageId AND userId=@userId AND code=@code;";

        private KeyValuePairInMemoryDatabase<long, LocalSQLite> _ConversationIdToLocalSQLiteKeyValuePairDatabase;
        private IIdentifierLock<long> _IdentifierLockConversationId;
        private string _DirectoryPath;
        public DalMessagesSQLiteSingleConversation(string directoryPath)
        {
            _DirectoryPath = directoryPath;
            Directory.CreateDirectory(directoryPath);
            _IdentifierLockConversationId= new IdentifierLock<long>();
            _ConversationIdToLocalSQLiteKeyValuePairDatabase = 
                new KeyValuePairInMemoryDatabase<long, LocalSQLite>(
                new OverflowParameters<long, LocalSQLite>(false,
                _IdentifierLockConversationId, OverflowLocalSQLite),
                _IdentifierLockConversationId);
        }
        public void AddReaction(long conversationId, MessageReaction reaction)
        {
            if (reaction.MultimediaToken == null)
            {
                UsingConnection(conversationId, (connection) =>
                {
                    using (SqliteCommand command = new SqliteCommand(
                        ADD_CODE_REACTION_COMMAND, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                        command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                        command.Parameters.Add(new SqliteParameter("@code", reaction.Code));
                        command.ExecuteNonQuery();
                    }
                });
            }
            else
            {
                UsingConnection(conversationId, (connection) =>
                {
                    using (SqliteCommand command = new SqliteCommand(
                        ADD_MULTIMEDIA_REACTION_COMMAND, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                        command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                        command.Parameters.Add(new SqliteParameter("@multimediaToken", reaction.MultimediaToken));
                        command.ExecuteNonQuery();
                    }
                });
            }
        }

        public void AddUserMultimediaItems(long conversationId, long messageId, UserMultimediaItem[] userMultimediaItems)
        {
            throw new NotImplementedException();
        }

        public void Append(long conversationId, ClientMessage message, out ClientMessage replyMessage)
        {
            string commandString = INSERT_COMMAND;
            bool isReply = message.ReplyTo != null;
            if (isReply)
                commandString += SELECT_INSERT_REPLY_COMMAND;
            ClientMessage replyMessageInternal = null;
            UsingConnection(conversationId, (connection) => {
                using (SqliteCommand command = new SqliteCommand(
                    commandString, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@id", message.Id));
                    command.Parameters.Add(new SqliteParameter("@version", message.Version));
                    command.Parameters.Add(new SqliteParameter("@sentAt", message.SentAt));
                    command.Parameters.Add(new SqliteParameter("@userId", message.UserId));
                    command.Parameters.Add(new SqliteParameter("@content", message.Content));
                    command.Parameters.Add(new SqliteParameter("@replyTo", isReply?message.ReplyTo: DBNull.Value));
                    if (!isReply)
                    {
                        command.ExecuteNonQuery();
                        return;
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            int version = reader.GetInt32(1);
                            long sentAt = reader.GetInt64(2);
                            long userId = reader.GetInt64(3);
                            string content = reader.GetString(4);
                            replyMessageInternal = new ClientMessage(id, version, sentAt, userId, content);
                        }
                    }
                }
            });
            replyMessage = replyMessageInternal;
        }
        public long[] Delete(long myUserId, long conversationId, long[] messageIds, out List<string> multimediaTokensDeleted)
        {
            throw new NotImplementedException("multimediaTokensDeleted");
            string commandString =
                    string.Format(DELETE_MESSAGES_COMMAND, myUserId, string.Join(',', messageIds));
            List<long> deletedIds = new List<long>();
            UsingConnection(conversationId, (connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    commandString, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            deletedIds.Add(id);
                        }
                    }
                    if (deletedIds.Count > 0)
                    {
                        command.Parameters.Clear();
                        string idsString = string.Join(',', deletedIds);
                        StringBuilder sb = new StringBuilder();
                        sb.Append(DELETE_REACTIONS_COMMAND_1);
                        sb.Append(idsString);
                        sb.Append(DELETE_REACTIONS_COMMAND_2);
                        sb.Append(idsString);
                        sb.Append(DELETE_REACTIONS_COMMAND_3);
                        command.CommandText = sb.ToString();
                        command.ExecuteScalar();
                    }
                }
            });
            return deletedIds.ToArray();
        }
        public long[] DeleteAny(long conversationId, long[] messageIds, out List<string> multimediaTokensDeleted)
        {
            throw new NotImplementedException("multimediaTokensDeleted");
            string commandString =
                    string.Format(DELETE_ANY_MESSAGES_COMMAND, string.Join(',', messageIds));
            List<long> deletedIds = new List<long>();
            UsingConnection(conversationId, (connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    commandString, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            deletedIds.Add(id);
                        }
                    }
                }
            });
            return deletedIds.ToArray();
        }
        public void Modify(long conversationId, ClientMessage message)
        {
            UsingConnection(conversationId, (connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    UPDATE_COMMAND, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@id", message.Id));
                    command.Parameters.Add(new SqliteParameter("@userId", message.UserId));
                    command.Parameters.Add(new SqliteParameter("@content", message.Content));
                    command.Parameters.Add(new SqliteParameter("@version", message.Version));
                    command.ExecuteNonQuery();
                }
            });
        }
        public ClientMessage[] ReadFromEnd(long conversationId, int nMessages, 
            out MessageReaction[]? reactions,
            out MessageUserMultimediaItem[]? messageUserMultimediaItemss)
        {
            List<ClientMessage> messages = new List<ClientMessage>(), repliesList = null;
            List<MessageReaction> reactionsList = null;
            UsingConnection(conversationId, (connection) => {

                using (SqliteCommand command = new SqliteCommand(
                    READ_FROM_END_COMMAND,
                    connection))
                {
                    command.Parameters.Add(new SqliteParameter("@nMessages", nMessages));
                    ReadClientMessages(command, messages);
                    command.CommandText = ConstructReadReactionsCommand(messages);
                    ReadMessageReactions(command, ref reactionsList);
                }
            });
            reactions = reactionsList?.ToArray();
            messageUserMultimediaItemss = null;
            throw new NotImplementedException();
            return ((IEnumerable<ClientMessage>)messages).Reverse().ToArray();
        }
        public ClientMessage[] ReadRange(long conversationId, int? nMessages,
            long? idFromInclusive, long? idToExclusive, out MessageReaction[] reactions)
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
            List<ClientMessage> messages = new List<ClientMessage>();
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
            List<MessageReaction> reactionsList = null;
            UsingConnection(conversationId, (connection) => {

                using (SqliteCommand command = new SqliteCommand(
                    commandString,
                    connection))
                {

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
                    ReadClientMessages(command, messages);
                    command.CommandText = ConstructReadReactionsCommand(messages);
                    ReadMessageReactions(command, ref reactionsList);
                }
            });
            reactions = reactionsList?.ToArray();
            if (reverse)
                return ((IEnumerable<ClientMessage>)messages).Reverse().ToArray();
            return messages.ToArray();
        }

        public ClientMessage[] ReadRange(long conversationId, int? nMessages, long? idFromInclusive, long? idToExclusive, out MessageReaction[] reactions, out MessageUserMultimediaItem[] messageUserMultimediaItemss)
        {
            throw new NotImplementedException();
        }

        public void RemoveReaction(long conversationId, MessageReaction reaction)
        {
            if (reaction.MultimediaToken == null)
            {
                UsingConnection(conversationId, (connection) =>
                {
                    using (SqliteCommand command = new SqliteCommand(
                        REMOVE_CODE_REACTION_COMMAND, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                        command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                        command.Parameters.Add(new SqliteParameter("@code", reaction.Code));
                        command.ExecuteNonQuery();
                    }
                });
            }
            else
            {
                UsingConnection(conversationId, (connection) =>
                {
                    using (SqliteCommand command = new SqliteCommand(
                        REMOVE_MULTIMEDIA_REACTION_COMMAND, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                        command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                        command.Parameters.Add(new SqliteParameter("@multimediaToken", reaction.MultimediaToken));
                        command.ExecuteNonQuery();
                    }
                });
            }
        }
        private string ConstructReadReactionsCommand(List<ClientMessage> messages) {
            string messageIdsString = string.Join(',', messages.Select(m => m.Id));
            StringBuilder sb = new StringBuilder(READ_MESSAGE_REACTIONS_1);
            sb.Append(messageIdsString);
            sb.Append(READ_MESSAGE_REACTIONS_2);
            sb.Append(messageIdsString);
            sb.Append(READ_MESSAGE_REACTIONS_3);
            return  sb.ToString();
        }
        private void OverflowLocalSQLite(long conversationId, LocalSQLite localSQLite)
        {
            localSQLite.Dispose();
        }
        private void PrepareConversationDatabase(SqliteConnection connection)
        {
            using (SqliteCommand command = new SqliteCommand(CREATE_COMMAND,
                connection))
            {
                command.ExecuteNonQuery();
            }
        }
        private void ReadClientMessages(SqliteCommand command, List<ClientMessage> messages)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    int version = reader.GetInt32(1);
                    long sentAt = reader.GetInt64(2);
                    long userId = reader.GetInt64(3);
                    string content = reader.GetString(4);
                    long? replyTo;
                    if (reader.IsDBNull(5))
                    {
                        messages.Add(new ClientMessage(id, version, sentAt, userId, content));
                    }
                    else
                    {
                        replyTo = reader.GetInt64(5);
                        long replyUserId = reader.GetInt64(6);
                        string replyContent = reader.GetString(7);
                        int replyVersion = reader.GetInt32(8);
                        ClientMessage replyMessage = new ClientMessage((long)replyTo, replyVersion, 0, replyUserId, replyContent);
                        messages.Add(new ClientMessage(id, version, sentAt, userId, content, replyMessage));
                    }
                }
            }
        }
        private void ReadMessageReactions(SqliteCommand command, ref List<MessageReaction> reactions)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    long messageId = reader.GetInt64(0);
                    long userId = reader.GetInt64(1);
                    if (reactions == null) reactions = new List<MessageReaction>();
                    reactions.Add(reader.IsDBNull(2)
                        ? new MessageReaction(messageId, userId, null, reader.GetString(3))
                        : new MessageReaction(messageId, userId, reader.GetInt32(2), null));
                }
            }
        }
        private void UsingConnection(long conversationId, Action<SqliteConnection> callback)
        {
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
                            string path = _DirectoryPath;
                            if(path.Last()!=Path.DirectorySeparatorChar)
                                path+= Path.DirectorySeparatorChar;
                            path += DatabasePathsHelper.SplitIdentifierIntoHundredsPathSegments(conversationId);
                            path += ".sqlite";
                            localSqlite = new LocalSQLite(path, true, maxNConnections: MAX_N_CONNECTIONS);
                            localSqlite.UsingConnectionForWrite(PrepareConversationDatabase);
                        }
                        handle = localSqlite.TakeConnectionDangerous();
                        return localSqlite;
                    });
                callback(handle.Object);
            }
            finally
            {
                handle?.Dispose();
            }
        }
    }
}