using Chat.Interfaces;
using Microsoft.Data.Sqlite;
using Core.Enums;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using System.Text;
using SQLite;
using System.Data;
using MultimediaCore;
using UsersEnums;
using MultimediaServerCore.Enums;
using Chat;
using Database;
using GlobalConstants;
using HashTags;
using HashTags.Messages;
using HashTags.Enums;
namespace Core.DAL
{
    public abstract class DalMessagesSQLiteMultipleConversationsShards : IDalMessages
    {
        private const int MAX_N_CONNECTIONS = 4;
        private const string
            SHORT_SELECTS = "id, version, sentAt, userId, content ",
            SELECTS = "a.id, a.version, a.sentAt, a.userId, a.content, a.replyTo, a.userId as replyUserId, b.content as replyContent, b.version as replyVersion ",
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblMessages (id INTEGER UNIQUE NOT NULL, conversationId INTEGER NOT NULL, version SMALL NOT NULL, " +
                    "sentAt INTEGER NOT NULL, userId INTEGER NOT NULL, content TEXT, replyTo INTEGER, tags TEXT);" +
                "CREATE INDEX IF NOT EXISTS ix_tblMessages_id ON tblMessages(id);" +
                "CREATE INDEX IF NOT EXISTS ix_tblMessages_conversationId_id ON tblMessages(conversationId, id);" +
                "CREATE INDEX IF NOT EXISTS ix_tblMessages_sentAt ON tblMessages(sentAt);" +
                "CREATE INDEX IF NOT EXISTS ix_tblMessages_userId ON tblMessages(userId);" +
                "CREATE TABLE IF NOT EXISTS tblReactionCodes (messageId INTEGER NOT NULL, " +
                    "userId INTEGER NOT NULL, code INTEGER NOT NULL, " +
                    "UNIQUE(messageId, userId, code));" +
                "CREATE INDEX IF NOT EXISTS ix_tblReactionCodes_messageId_userId ON tblReactionCodes(messageId,userId);" +
                "CREATE TABLE IF NOT EXISTS tblReactionMultimediaTokens (messageId INTEGER NOT NULL, " +
                    "userId INTEGER NOT NULL, multimediaToken TEXT NOT NULL, " +
                    "UNIQUE(messageId, userId, multimediaToken));" +
                "CREATE INDEX IF NOT EXISTS ix_tblReactionMultimediaTokens_messageId_userId " +
                    "ON tblReactionMultimediaTokens(messageId,userId);" +
                "CREATE TABLE IF NOT EXISTS tblUserMultimediaItems (messageId INTEGER NOT NULL, status SMALL NOT NULL, " +
                    "multimediaToken TEXT NOT NULL, xRating SMALL, description TEXT);" +
                "CREATE INDEX IF NOT EXISTS ix_tblUserMultimediaItems_messageId " +
                    "ON tblUserMultimediaItems(messageId);"+
                "CREATE TABLE IF NOT EXISTS tblChildConversations(messageId INTEGER PRIMARY KEY, " +
                    "conversationId INTEGER NOT NULL, nChildMessages INTEGER);",
            WHERE_CONVERSATION_ID = "WHERE a.conversationId = @conversationId ",
            REPLIES_LEFT_JOIN = "LEFT OUTER JOIN tblMessages b on a.replyTo = b.id ",
            _READ_FROM_END_COMMAND = "SELECT " + SELECTS +
                "{0}" + REPLIES_LEFT_JOIN + WHERE_CONVERSATION_ID + "ORDER BY a.id DESC LIMIT @nMessages;",
            _READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND = "SELECT " + SELECTS +
                "{0}" + REPLIES_LEFT_JOIN + WHERE_CONVERSATION_ID + "AND a.id < @idToExclusive ORDER BY a.id DESC LIMIT @nMessages;",
            _READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND = "SELECT " + SELECTS +
                "{0}" + REPLIES_LEFT_JOIN + WHERE_CONVERSATION_ID + "AND a.id >= @idFromInclusive ORDER BY a.id LIMIT @nMessages;",
            _READ_EACH_SIDE_OF_ID_COMMAND = "SELECT * FROM (SELECT " + SELECTS +
                "{0}" + REPLIES_LEFT_JOIN + WHERE_CONVERSATION_ID + "AND a.id < @id ORDER BY a.id DESC LIMIT @nMessagesEitherSide) " +
                "UNION SELECT * FROM (SELECT " + SELECTS +
                "{1}" +REPLIES_LEFT_JOIN+ WHERE_CONVERSATION_ID + "AND a.id >= @id ORDER BY a.id LIMIT @nMessagesEitherSide) ORDER BY id;",
            _READ_BETWEEN_IDS_COMMAND = "SELECT " + SELECTS +
                "{0}" + REPLIES_LEFT_JOIN + WHERE_CONVERSATION_ID + "AND a.id < @idToExclusive AND a.id>=@idFromInclusive ORDER BY a.id LIMIT @nMessages;",


            INSERT_COMMAND = "INSERT INTO tblMessages (id,conversationId,version,sentAt,userId,content,replyTo,tags) " +
            "VALUES(@id,@conversationId,@version,@sentAt,@userId,@content,@replyTo,@tags);",
            ADD_USER_MULTIMEDIA_ITEM_COMMAND = "INSERT INTO tblUserMultimediaItems (messageId,multimediaToken,status,xRating,description) " +
            "VALUES(@messageId,@multimediaToken,@status,@xRating,@description);",
            SELECT_INSERT_REPLY_COMMAND = "SELECT " + SHORT_SELECTS + "FROM tblMessages WHERE id=@replyTo;",
            DELETE_MESSAGES_COMMAND = "DELETE FROM tblMessages WHERE userId={0} AND id IN ({1}) RETURNING id ",
            DELETE_ANY_MESSAGES_COMMAND = "DELETE FROM tblMessages WHERE id IN ({0}) RETURNING id ",
            DELETE_REACTIONS_COMMAND_1 = "DELETE FROM tblReactionCodes WHERE messageId IN (",
            DELETE_REACTIONS_COMMAND_2 = "); DELETE FROM tblReactionMultimediaTokens WHERE messageId IN (",
            DELETE_REACTIONS_COMMAND_3 = ");",
            DELETE_USER_MULTIMEDIA_ITEMS_COMMAND_1 = "DELETE FROM tblUserMultimediaItems WHERE messageId IN (",
            DELETE_USER_MULTIMEDIA_ITEMS_COMMAND_2 = ") RETURNING tblUserMultimediaItems.multimediaToken;",
            UPDATE_COMMAND = "UPDATE tblMessages " +
            "SET content=@content,version=@version,tags=@tags WHERE id= @id AND userId=@userId",
            ADD_CODE_REACTION_COMMAND = "INSERT OR IGNORE INTO tblReactionCodes(messageId, userId, code) VALUES(@messageId, @userId, @code)",
            ADD_MULTIMEDIA_REACTION_COMMAND = "INSERT OR IGNORE INTO tblReactionMultimediaTokens(messageId, userId, multimediaToken) VALUES(@messageId, @userId, @multimediaToken)",
            READ_MESSAGE_REACTIONS_1 =
                "SELECT * FROM (SELECT messageId, userId, code, null as multimediaToken FROM tblReactionCodes " +
                "WHERE messageId IN (",
            READ_MESSAGE_REACTIONS_2 = ")) UNION SELECT * FROM (" +
            "SELECT messageId, userId, null as code, multimediaToken FROM tblReactionMultimediaTokens WHERE messageId IN (",
            READ_MESSAGE_REACTIONS_3 = "));",

            READ_MESSAGE_USER_MULTIMEDIA_ITEMS_S_1 =
                "SELECT messageId, multimediaToken, status, xRating, description FROM tblUserMultimediaItems " +
                "WHERE messageId IN (",
            READ_MESSAGE_USER_MULTIMEDIA_ITEMS_S_2 = ");",
            READ_INDIVIDUAL_MESSAGE_COMMAND= "SELECT " + SHORT_SELECTS + "FROM tblMessages WHERE conversationId = @conversationId "
                + "AND id = @messageId;",


            REMOVE_MULTIMEDIA_REACTION_COMMAND = "DELETE FROM tblReactionMultimediaTokens WHERE messageId=@messageId AND userId=@userId AND multimediaToken=@multimediaToken;",
            REMOVE_CODE_REACTION_COMMAND = "DELETE FROM tblReactionCodes WHERE messageId=@messageId AND userId=@userId AND code=@code;",
            _READ__WITHOUT_ASSOCIATED_CONVERSATIONS = "FROM tblMessages a ",
            _READ__WITH_ASSOCIATED_CONVERSATIONS= ", c.conversationId as childConversationId, c.nChildMessages as nChildMessages FROM tblMessages a LEFT OUTER JOIN tblChildConversations c on a.id = c.messageId ",
            GET_MESSAGE_CHILD_CONVERSATION_ID_COMMAND = "SELECT conversationId FROM tblChildConversations WHERE messageId = @messageId;",
            SET_MESSAGE_CHILD_CONVERSATION_ID_COMMAND = "INSERT INTO tblChildConversations(messageId, conversationId) VALUES(@messageId, @conversationId);",
            ADD_TBLMESSAGES_TAGS="ALTER TABLE tblMessages ADD tags TEXT;",
            GET_TAGS_FROM_EXISTING_MESSAGE="SELECT tags FROM tblMessages WHERE id = @messageId;";
            
        private static readonly string
            READ_FROM_END_COMMAND = string.Format(_READ_FROM_END_COMMAND, _READ__WITHOUT_ASSOCIATED_CONVERSATIONS),
            READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND = string.Format(_READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND, _READ__WITHOUT_ASSOCIATED_CONVERSATIONS),
            READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND = string.Format(_READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND, _READ__WITHOUT_ASSOCIATED_CONVERSATIONS),
            READ_EACH_SIDE_OF_ID_COMMAND = string.Format(_READ_EACH_SIDE_OF_ID_COMMAND, _READ__WITHOUT_ASSOCIATED_CONVERSATIONS, _READ__WITHOUT_ASSOCIATED_CONVERSATIONS),
            READ_BETWEEN_IDS_COMMAND = string.Format(_READ_BETWEEN_IDS_COMMAND, _READ__WITHOUT_ASSOCIATED_CONVERSATIONS),

            READ_FROM_END_WITH_ASSOCIATED_CONVERSATIONS_COMMAND = string.Format(_READ_FROM_END_COMMAND, _READ__WITH_ASSOCIATED_CONVERSATIONS),
            READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_WITH_ASSOCIATED_CONVERSATIONS_COMMAND = string.Format(_READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND, _READ__WITH_ASSOCIATED_CONVERSATIONS),
            READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_WITH_ASSOCIATED_CONVERSATIONS_COMMAND = string.Format(_READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND, _READ__WITH_ASSOCIATED_CONVERSATIONS),
            READ_EACH_SIDE_OF_ID_WITH_ASSOCIATED_CONVERSATIONS_COMMAND = string.Format(_READ_EACH_SIDE_OF_ID_COMMAND, _READ__WITH_ASSOCIATED_CONVERSATIONS, _READ__WITH_ASSOCIATED_CONVERSATIONS),
            READ_BETWEEN_IDS_WITH_ASSOCIATED_CONVERSATIONS_COMMAND = string.Format(_READ_BETWEEN_IDS_COMMAND, _READ__WITH_ASSOCIATED_CONVERSATIONS);

        private LocalSQLiteShards _Shards;
        protected abstract HashTagScopeTypes _HashTagScopeType { get; }
        protected DalMessagesSQLiteMultipleConversationsShards(string directoryPath, int shardSize)
        {
            Directory.CreateDirectory(directoryPath);
            _Shards = new LocalSQLiteShards(
                directoryPath, true, shardSize,
                PrepareConversationDatabase,
                maxNConnections: MAX_N_CONNECTIONS);
        }
        public void AddReaction(long conversationId, MessageReaction reaction)
        {
            if (reaction.MultimediaToken == null)
            {
                _Shards.UsingConnectionForWrite(conversationId, (connection) =>
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            ADD_CODE_REACTION_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                            command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                            command.Parameters.Add(new SqliteParameter("@code", reaction.Code));
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                });
            }
            else
            {
                _Shards.UsingConnectionForWrite(conversationId, (connection) =>
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            ADD_MULTIMEDIA_REACTION_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                            command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                            command.Parameters.Add(new SqliteParameter("@multimediaToken", reaction.MultimediaToken));
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                });
            }
        }
        public void Append(long conversationId, ClientMessage message, out ClientMessage replyMessage)
        {
            string commandString = INSERT_COMMAND;
            bool isReply = message.ReplyTo != null;
            if (isReply)
                commandString += SELECT_INSERT_REPLY_COMMAND;
            ClientMessage replyMessageInternal = null;
            _Shards.UsingConnectionForWrite(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        commandString, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@id", message.Id));
                        command.Parameters.Add(new SqliteParameter("@conversationId", conversationId));
                        command.Parameters.Add(new SqliteParameter("@version", message.Version));
                        command.Parameters.Add(new SqliteParameter("@sentAt", message.SentAt));
                        command.Parameters.Add(new SqliteParameter("@userId", message.UserId));
                        command.Parameters.Add(new SqliteParameter("@content", message.Content==null?DBNull.Value:message.Content));
                        command.Parameters.Add(new SqliteParameter("@replyTo", isReply ? message.ReplyTo : DBNull.Value));
                        command.Parameters.Add(new SqliteParameter("@tags", message.Tags!=null&&message.Tags.Any() 
                            ? string.Join(',', message.Tags): DBNull.Value));
                        if (!isReply)
                        {
                            command.ExecuteNonQuery();
                            transaction.Commit();
                            return;
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long id = reader.GetInt64(0);
                                int version = reader.GetInt32(1);
                                long sentAt = reader.GetInt64(2);
                                long userId = reader.GetInt64(3);
                                string content = reader.GetString(4);
                                replyMessageInternal = new ClientMessage(id, version, sentAt, userId, content);
                            }
                        }
                        transaction.Commit();
                    }
                }
            });
            AddNewMessageTagsToHashTagsSystem(conversationId, message);
            if (message.UserMultimediaItems != null && message.UserMultimediaItems.Any())
            {
                AddUserMultimediaItems(message.ConversationId,
                    message.Id, message.UserMultimediaItems);
            }
            replyMessage = replyMessageInternal;
        }
        public long[] Delete(long myUserId, long conversationId, long[] messageIds, out List<string> multimediaTokensDeleted)
        {
            string commandString =
                    string.Format(DELETE_MESSAGES_COMMAND, myUserId, string.Join(',', messageIds));
            List<long> deletedIds = new List<long>();
            List<string> multimediaTokensDeletedInternal = null;
            _Shards.UsingConnectionForWrite(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        commandString, connection, transaction))
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
                            DeleteReactions(command, deletedIds);
                            DeleteUserMultimediaItems(command, deletedIds, out multimediaTokensDeletedInternal);
                        }
                        transaction.Commit();
                    }
                }
            });
            multimediaTokensDeleted = multimediaTokensDeletedInternal;
            return deletedIds.ToArray();
        }
        public long[] DeleteAny(long conversationId, long[] messageIds, out List<string> multimediaTokensDeleted)
        {
            string commandString =
                    string.Format(DELETE_ANY_MESSAGES_COMMAND, string.Join(',', messageIds));
            List<long> deletedIds = new List<long>();
            List<string> multimediaTokensDeletedInternal = null;
            _Shards.UsingConnectionForWrite(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        commandString, connection, transaction))
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
                            DeleteReactions(command, deletedIds);
                            DeleteUserMultimediaItems(command, deletedIds, out multimediaTokensDeletedInternal);
                        }
                        transaction.Commit();
                    }
                }
            });
            multimediaTokensDeleted = multimediaTokensDeletedInternal;
            return deletedIds.ToArray();
        }
        public long? GetChildConversationIdForMessage(long parentConversationId, long messageId) {
            return _Shards.UsingConnection(parentConversationId, (connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    GET_MESSAGE_CHILD_CONVERSATION_ID_COMMAND, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@messageId", messageId));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (long?)reader.GetInt64(0);
                        }
                        return null;
                    }
                }
            });
        }
        public void SetChildConversationIdForMessage(long parentConversationId, long messageId, long conversationId)
        {
            _Shards.UsingConnectionForWrite(parentConversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        SET_MESSAGE_CHILD_CONVERSATION_ID_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", messageId));
                        command.Parameters.Add(new SqliteParameter("@conversationId", conversationId));
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            });
        }
        private void DeleteReactions(SqliteCommand command, List<long> deletedIds)
        {
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
        public void AddUserMultimediaItems(long conversationId, long messageId,
            UserMultimediaItem[] userMultimediaItems)
        {
            _Shards.UsingConnectionForWrite(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        ADD_USER_MULTIMEDIA_ITEM_COMMAND, connection, transaction))
                    {
                        foreach (var item in userMultimediaItems)
                        {
                            command.Parameters.Add(new SqliteParameter("@messageId", messageId));
                            command.Parameters.Add(new SqliteParameter("@multimediaToken", item.MultimediaToken));
                            command.Parameters.Add(new SqliteParameter("@status", item.Status));
                            command.Parameters.Add(new SqliteParameter("@xRating", item.XRating));
                            command.Parameters.Add(new SqliteParameter("@description", item.Description == null ? DBNull.Value : item.Description));
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }
                    transaction.Commit();
                }
            });
        }
        private void DeleteUserMultimediaItems(SqliteCommand command, IEnumerable<long> deletedIds, out List<string> multimediaTokensDeleted)
        {
            string idsString = string.Join(',', deletedIds);
            StringBuilder sb = new StringBuilder();
            sb.Append(DELETE_USER_MULTIMEDIA_ITEMS_COMMAND_1);
            sb.Append(idsString);
            sb.Append(DELETE_USER_MULTIMEDIA_ITEMS_COMMAND_2);
            command.CommandText = sb.ToString();
            multimediaTokensDeleted = null;
            using (var reader = command.ExecuteReader()) {
                while (reader.Read())
                {
                    string multimediaToken = reader.GetString(0);
                    if (multimediaTokensDeleted == null)
                        multimediaTokensDeleted = new List<string> { multimediaToken };
                    else
                        multimediaTokensDeleted.Add(multimediaToken);
                }
            }
        }
        public void Modify(long conversationId, ClientMessage message, out List<string> multimediaTokensDeleted)
        {
            bool updated = false;
            string[] existingTags = GetTagsFromExistingMessage(conversationId, message.Id);
            List<string>? multimediaTokensDeletedInternal = null;
            _Shards.UsingConnectionForWrite(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        UPDATE_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@id", message.Id));
                        command.Parameters.Add(new SqliteParameter("@userId", message.UserId));
                        command.Parameters.Add(new SqliteParameter("@content", message.Content == null ? DBNull.Value : message.Content));
                        command.Parameters.Add(new SqliteParameter("@version", message.Version));
                        command.Parameters.Add(new SqliteParameter("@tags", message.Tags != null && message.Tags.Any()
                            ? string.Join(',', message.Tags) : DBNull.Value));
                        updated = command.ExecuteNonQuery()>0;
                        DeleteUserMultimediaItems(
                            command, new long[] { message.Id },
                            out multimediaTokensDeletedInternal);
                        transaction.Commit();
                    }
                }
            });
            if (message.UserMultimediaItems != null && message.UserMultimediaItems.Any())
            {
                AddUserMultimediaItems(conversationId, message.Id, message.UserMultimediaItems);
                multimediaTokensDeleted = multimediaTokensDeletedInternal
                    ?.Where(m =>
                        !message.UserMultimediaItems
                        .Where(n => n.MultimediaToken == m)
                        .Any()
                    )
                    .ToList();
            }
            else {
                multimediaTokensDeleted = multimediaTokensDeletedInternal;
            }
            if(updated)
                UpdateHashTags(conversationId, message, existingTags);

        }
        public ClientMessage[] ReadFromEnd(long conversationId, int nMessages, out MessageReaction[]? reactions,
            out MessageUserMultimediaItem[]? messageUserMultimediaItemss, 
            MessageChildConversationOptions messageChildConversationOptions)
        {
            bool withChildConversations = messageChildConversationOptions != null;
            List<ClientMessage> messages = new List<ClientMessage>();
            List<MessageReaction> reactionsList = null;
            List<MessageUserMultimediaItem> messageUserMultimediaItemssList = null;
            _Shards.UsingConnection(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        withChildConversations
                        ?READ_FROM_END_WITH_ASSOCIATED_CONVERSATIONS_COMMAND
                        :READ_FROM_END_COMMAND,
                        connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@nMessages", nMessages));
                        command.Parameters.Add(new SqliteParameter("@conversationId", conversationId));
                        ReadClientMessages(command, messages, withChildConversations);
                        command.CommandText = ConstructReadReactionsCommand(messages);
                        ReadMessageReactions(command, ref reactionsList);
                        command.CommandText = ConstructReadMessageUserMultimediaItemssCommand(messages);
                        ReadMessageUserMultimediaItemss(command, ref messageUserMultimediaItemssList);
                    }
                }
            });
            reactions = reactionsList?.ToArray();
            messageUserMultimediaItemss = messageUserMultimediaItemssList?.ToArray();
            return ((IEnumerable<ClientMessage>)messages).Reverse().ToArray();
        }
        public ClientMessage[] ReadIndividualMessages(long conversationId, long[] messageIds)
        {
            List<ClientMessage> messages = new List<ClientMessage>(messageIds.Length);
            _Shards.UsingConnection(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        READ_INDIVIDUAL_MESSAGE_COMMAND,
                        connection, transaction))
                    {
                        foreach (long messageId in messageIds)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqliteParameter("@messageId", messageId));
                            command.Parameters.Add(new SqliteParameter("@conversationId", conversationId));
                            ClientMessage message = ReadClientMessage(command);
                            if (message != null)
                                messages.Add(message);
                        }
                    }
                }
            });
            return messages.ToArray();
        }
        public ClientMessage[] ReadRange(long conversationId, int? nMessages,
            long? idFromInclusive, long? idToExclusive, out MessageReaction[]? reactions,
            out MessageUserMultimediaItem[]? messageUserMultimediaItemss,
            MessageChildConversationOptions messageChildConversationOptions)
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
            bool withChildConversations = messageChildConversationOptions != null;
            switch (rangeType)
            {
                case RangeType.ToToExclusive:
                    commandString = withChildConversations
                        ? READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_WITH_ASSOCIATED_CONVERSATIONS_COMMAND 
                        : READ_BACKWARDS_FROM_ID_TO_EXCLUSIVE_COMMAND;
                    reverse = true;
                    break;
                case RangeType.FromFromInclusive:
                    commandString = withChildConversations
                        ? READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_WITH_ASSOCIATED_CONVERSATIONS_COMMAND
                        : READ_FORWARDS_FROM_ID_FROM_INCLUSIVE_COMMAND;
                    break;
                case RangeType.Between:
                    commandString = withChildConversations
                        ? READ_BETWEEN_IDS_WITH_ASSOCIATED_CONVERSATIONS_COMMAND
                        : READ_BETWEEN_IDS_COMMAND;
                    break;
                case RangeType.EitherSideOf:
                    commandString = withChildConversations
                        ? READ_EACH_SIDE_OF_ID_WITH_ASSOCIATED_CONVERSATIONS_COMMAND
                        : READ_EACH_SIDE_OF_ID_COMMAND;
                    break;
                default:
                case RangeType.Default:
                    commandString = withChildConversations
                        ? READ_FROM_END_WITH_ASSOCIATED_CONVERSATIONS_COMMAND
                        : READ_FROM_END_COMMAND;
                    reverse = true;
                    break;
            }
            List<MessageReaction> reactionsList = null;
            List<MessageUserMultimediaItem> messageUserMultimediaItemssList = null;
            _Shards.UsingConnection(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        commandString,
                        connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@conversationId", conversationId));
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
                        ReadClientMessages(command, messages, withChildConversations);
                        command.CommandText = ConstructReadReactionsCommand(messages);
                        ReadMessageReactions(command, ref reactionsList);
                        command.CommandText = ConstructReadMessageUserMultimediaItemssCommand(messages);
                        ReadMessageUserMultimediaItemss(command, ref messageUserMultimediaItemssList);
                    }
                }
            });
            reactions = reactionsList?.ToArray();
            messageUserMultimediaItemss = messageUserMultimediaItemssList?.ToArray();
            if (reverse)
                return ((IEnumerable<ClientMessage>)messages).Reverse().ToArray();
            return messages.ToArray();
        }
        public void RemoveReaction(long conversationId, MessageReaction reaction)
        {
            if (reaction.MultimediaToken == null)
            {
                _Shards.UsingConnectionForWrite(conversationId, (connection) =>
                {
                    using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            REMOVE_CODE_REACTION_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                            command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                            command.Parameters.Add(new SqliteParameter("@code", reaction.Code));
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                });
            }
            else
            {
                _Shards.UsingConnectionForWrite(conversationId, (connection) =>
                {
                    using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqliteCommand command = new SqliteCommand(
                            REMOVE_MULTIMEDIA_REACTION_COMMAND, connection, transaction))
                        {
                            command.Parameters.Add(new SqliteParameter("@messageId", reaction.MessageId));
                            command.Parameters.Add(new SqliteParameter("@userId", reaction.UserId));
                            command.Parameters.Add(new SqliteParameter("@multimediaToken", reaction.MultimediaToken));
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                });
            }
        }
        public string[] GetTagsFromExistingMessage(long conversationId, long messageId)
        {
            return _Shards.UsingConnection(conversationId, (connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        GET_TAGS_FROM_EXISTING_MESSAGE, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@messageId", messageId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return null;
                            }
                            return reader.IsDBNull(0) ? null:reader.GetString(0).Split(',');
                        }
                    }
                }
            });
        }
        private string ConstructReadReactionsCommand(List<ClientMessage> messages)
        {
            string messageIdsString = string.Join(',', messages.Select(m => m.Id));
            StringBuilder sb = new StringBuilder(READ_MESSAGE_REACTIONS_1);
            sb.Append(messageIdsString);
            sb.Append(READ_MESSAGE_REACTIONS_2);
            sb.Append(messageIdsString);
            sb.Append(READ_MESSAGE_REACTIONS_3);
            return sb.ToString();
        }
        private string ConstructReadMessageUserMultimediaItemssCommand(List<ClientMessage> messages)
        {
            string messageIdsString = string.Join(',', messages.Select(m => m.Id));
            StringBuilder sb = new StringBuilder(READ_MESSAGE_USER_MULTIMEDIA_ITEMS_S_1);
            sb.Append(messageIdsString);
            sb.Append(READ_MESSAGE_USER_MULTIMEDIA_ITEMS_S_2);
            return sb.ToString();
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
            if (!TableManagementHelper.ColumnExists(connection, "tblMessages", "tags"))
            {
                using (SqliteCommand command = new SqliteCommand(ADD_TBLMESSAGES_TAGS,
                    connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        private void ReadClientMessages(SqliteCommand command, List<ClientMessage> messages, bool withChildConversations)
        {
            using (var reader = command.ExecuteReader())
            {
                if (withChildConversations)
                {
                    while (reader.Read())
                    {
                        ClientMessage message = ReadClientMessage_Normal(reader);
                        messages.Add(message);
                        message.ChildConversationId = reader.GetInt64(9);
                        message.NChildMessages = reader.IsDBNull(10)?null:reader.GetInt32(10);
                    }
                }
                else { 
                    while (reader.Read())
                    {
                        messages.Add(ReadClientMessage_Normal(reader));
                    }
                }
            }
        }
        private ClientMessage ReadClientMessage(SqliteCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    int version = reader.GetInt32(1);
                    long sentAt = reader.GetInt64(2);
                    long userId = reader.GetInt64(3);
                    string content = reader.IsDBNull(4) ? null : reader.GetString(4);
                    return new ClientMessage(id, version, sentAt, userId, content);
                }
                return null;    
            }
        }
        private ClientMessage ReadClientMessage_Normal(SqliteDataReader reader) {

            long id = reader.GetInt64(0);
            int version = reader.GetInt32(1);
            long sentAt = reader.GetInt64(2);
            long userId = reader.GetInt64(3);
            string content = reader.IsDBNull(4) ? null : reader.GetString(4);
            long? replyTo;
            if (reader.IsDBNull(5))
            {
                return new ClientMessage(id, version, sentAt, userId, content);
            }
            replyTo = reader.GetInt64(5);
            long replyUserId = reader.GetInt64(6);
            string replyContent = reader.GetString(7);
            int replyVersion = reader.GetInt32(8);
            ClientMessage replyMessage = new ClientMessage((long)replyTo, replyVersion, 0, replyUserId, replyContent);
            return new ClientMessage(id, version, sentAt, userId, content, replyMessage);
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
        private void ReadMessageUserMultimediaItemss(SqliteCommand command, ref List<MessageUserMultimediaItem> messageUserMultimediaItemss)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (messageUserMultimediaItemss == null) messageUserMultimediaItemss = new List<MessageUserMultimediaItem>();
                    messageUserMultimediaItemss.Add(
                        new MessageUserMultimediaItem(reader.GetInt64(0), reader.GetString(1), (MultimediaItemStatus)reader.GetInt16(2), (XRating)reader.GetInt32(3), reader.IsDBNull(4)?null:reader.GetString(4)));
                }
            }
        }
        private void UpdateHashTags(
            long conversationId, ClientMessage newMessage, string[] existingTags)
        {
            if (existingTags != null && existingTags.Any())
            {
                UpdateExistingTagsOnHashTagsSystem(conversationId, newMessage, existingTags);
                return;
            }
            AddNewMessageTagsToHashTagsSystem(conversationId, newMessage);
        }
        private void UpdateExistingTagsOnHashTagsSystem(long conversationId, ClientMessage newMessage,
            string[] existingTags) {
            HashTagsHelper.CrossCompareTags(newMessage.Tags, existingTags, out string[] tagsToRemove, out string[] tagsToAdd);
            if (tagsToRemove != null&&tagsToRemove.Any())
            {
                HashTagsMesh.Instance.DeleteTags(
                    _HashTagScopeType, conversationId, newMessage.Id, tagsToRemove);
            }
            if (tagsToAdd != null && tagsToAdd.Any())
            {
                HashTagsMesh.Instance.AddTags(
                    tagsToAdd, HashTagScopeTypes.ChatRoomMessage, conversationId, newMessage.Id);
            }
        }
        private void AddNewMessageTagsToHashTagsSystem(long conversationId, ClientMessage newMessage) {
            if (newMessage.Tags != null && newMessage.Tags.Any())
            {
                HashTagsMesh.Instance.AddTags(newMessage.Tags, _HashTagScopeType,
                    conversationId, newMessage.Id);
            }
        }
    }
}