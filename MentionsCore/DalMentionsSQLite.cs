using Core.Exceptions;
using Database;
using Microsoft.Data.Sqlite;
using MentionsCore.Messages;
using System.Data;
using DependencyManagement;
using MentionsCore;
namespace Core.DAL
{
    public class DalMentionsSQLite
    {
        private const string
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblMentions (userIdBeingMentioned INTEGER NOT NULL," +
                "userIdMentioning INTEGER NOT NULL, atTime INTEGER NOT NULL, messageId INTEGER NOT NULL," +
                "conversationId INTEGER NOT NULL, contentSnapshot TEXT NOT NULL, seen BOOLEAN);" +
                "CREATE INDEX IF NOT EXISTS index_userIdMentioning_messageId ON tblMentions (userIdMentioning, messageId);" +
            //having a user id before message id in a composite index increases index size but means it doesnt have to iterate through messages to see if user mentioned.
                "CREATE INDEX IF NOT EXISTS index_userIdBeingMentioned_messageId ON tblMentions (userIdBeingMentioned, messageId);" + 
                "CREATE INDEX IF NOT EXISTS index_atTime ON tblMentions (atTime);",
            SELECTS = " userIdMentioning, atTime, messageId, conversationId, contentSnapshot, seen ",
            GET_COMMAND = 
                "SELECT" +SELECTS+
                "FROM tblMentions WHERE userIdBeingMentioned = @userIdBeingMentioned " +
                "ORDER BY messageId DESC LIMIT @nEntries;",
            GET_UP_TO_ID_EXCLUSIVE_COMMAND = 
                "SELECT" +SELECTS +
                "FROM tblMentions WHERE userIdBeingMentioned = @userIdBeingMentioned AND messageId < @toIdExclusive " +
                "ORDER BY messageId DESC LIMIT @nEntries;",
            GET_UP_FROM_ID_INCLUSIVE_COMMAND = 
                "SELECT " + SELECTS +
                "FROM tblMentions WHERE userIdBeingMentioned = @userIdBeingMentioned AND messageId >= @fromIdInclusive " +
                "ORDER BY messageId LIMIT @nEntries;",
            INSERT_COMMAND = 
                "INSERT INTO tblMentions (userIdBeingMentioned,userIdMentioning,atTime,messageId," +
                    "conversationId,contentSnapshot,seen) " +
                "VALUES(@userIdBeingMentioned,@userIdMentioning,@atTime,@messageId," +
                    "@conversationId,@contentSnapshot,@seen);",
            SET_SEEN_COMMAND =
                "UPDATE tblMentions set seen = TRUE where userIdBeingMentioned = @userIdBeingMentioned AND messageId = @messageId;",
            DELETE_EXISTING_FOR_MESSAGE_COMMAND = 
                "DELETE FROM tblMentions WHERE messageId = @messageId;",
            DELETE_ALL_FOR_USER_COMMAND =
                //userIdMentioning included for composite index. Do not remove this.
                "DELETE FROM tblMentions WHERE userIdMentioning=@userIdMentioning;",
            ADD_TBLMENTIONS_SEEN = "ALTER TABLE tblMentions ADD seen BOOLEAN;",
            DELETE_UP_TO_TIME_COMMAND = 
                "DELETE FROM tblMentions WHERE atTime < @atTimeTo;";
        private static DalMentionsSQLite? _Instance;
        public static DalMentionsSQLite Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMentionsSQLite));
                return _Instance;
            }
        }
        public static DalMentionsSQLite Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMentionsSQLite));
            _Instance = new DalMentionsSQLite();
            return _Instance;
        }


        private LocalSQLite _LocalSQLite;
        private DalMentionsSQLite()
        {
            _LocalSQLite = new LocalSQLite(DependencyManager.GetString(DependencyNames.MentionsSqliteLocalDatabaseFilePath), true);
            /*TODO may need to shard this locally because of limitations on cocurrent writes.
            Though lets deal with this when it becomes an issue. As cocurrent writes on a single
            edge may never be enough to cause a problem.*/
            _LocalSQLite.UsingConnection((connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    CREATE_COMMAND, connection))
                {
                    command.ExecuteNonQuery();
                }
                if (!TableManagementHelper.ColumnExists(connection, "tblMentions", "seen"))
                {
                    using (SqliteCommand command = new SqliteCommand(ADD_TBLMENTIONS_SEEN,
                        connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            });
        }
        public void Add(long[] userIdBeingMentioneds, Mention mention, bool isUpdate)
        {
            _LocalSQLite.UsingConnection((connection) => {
                using(var transaction =  connection.BeginTransaction(IsolationLevel.ReadUncommitted)) {
                    using (SqliteCommand command = new SqliteCommand(null, connection, transaction))
                    {
                        if (isUpdate)
                        {
                            command.CommandText = DELETE_EXISTING_FOR_MESSAGE_COMMAND;
                            command.Parameters.Add(new SqliteParameter("@userIdMentioning", mention.UserIdMentioning));
                            command.Parameters.Add(new SqliteParameter("@messageId", mention.MessageId));
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                        foreach (long userIdBeingMentioned in userIdBeingMentioneds)
                        {
                            command.CommandText = INSERT_COMMAND;
                            command.Parameters.Add(new SqliteParameter("@userIdBeingMentioned", userIdBeingMentioned));
                            command.Parameters.Add(new SqliteParameter("@userIdMentioning", mention.UserIdMentioning));
                            command.Parameters.Add(new SqliteParameter("@atTime", mention.AtTime));
                            command.Parameters.Add(new SqliteParameter("@messageId", mention.MessageId));
                            command.Parameters.Add(new SqliteParameter("@conversationId", mention.ConversationId));
                            command.Parameters.Add(new SqliteParameter("@contentSnapshot", mention.ContentSnapshot));
                            command.Parameters.Add(new SqliteParameter("@seen", mention.Seen));
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                        transaction.Commit();
                    }
                }
            });
        }
        public void DeleteUpAtTimeTo(long atTimeTo)
        {
            _LocalSQLite.UsingConnection((connection) => {
                using (SqliteCommand command = new SqliteCommand(
                    DELETE_UP_TO_TIME_COMMAND, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@atTimeTo", atTimeTo));
                    command.ExecuteNonQuery();
                }
            });
        }
        public Mention[] Get(long userIdBeingMentioned, int nEntries, long? toIdExclusive,
            long? fromIdInclusive)
        {
            List<Mention> mentions = new List<Mention>();
            string commandString = toIdExclusive != null 
                ? GET_UP_TO_ID_EXCLUSIVE_COMMAND 
                : (fromIdInclusive!=null? GET_UP_FROM_ID_INCLUSIVE_COMMAND:GET_COMMAND);
            _LocalSQLite.UsingConnection((connection) => {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(commandString, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@nEntries", nEntries));
                        command.Parameters.Add(new SqliteParameter("@userIdBeingMentioned", userIdBeingMentioned));
                        if (toIdExclusive != null)
                            command.Parameters.Add(new SqliteParameter("@toIdExclusive", (long)toIdExclusive));
                        else if (fromIdInclusive != null)
                            command.Parameters.Add(new SqliteParameter("@fromIdInclusive", (long)fromIdInclusive));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mentions.Add(new Mention(
                                    reader.GetInt64(0), 
                                    reader.GetInt64(1), 
                                    reader.GetInt64(2),
                                    reader.GetInt64(3), 
                                    reader.GetString(4), 
                                    reader.IsDBNull(5)?false:reader.GetBoolean(5)));
                            }
                        }
                    }
                }
            });
            return mentions.ToArray();
        }
        public void SetSeen(long userIdBeingMentioned, long messageId)
        {
            _LocalSQLite.UsingConnection((connection) => {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(SET_SEEN_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@userIdBeingMentioned", userIdBeingMentioned));
                        command.Parameters.Add(new SqliteParameter("@messageId", messageId));
                        command.ExecuteScalar();
                        transaction.Commit();
                    }
                }
            });
        }
    }
}