using Core.Exceptions;
using Core.Timing;
using Database;
using Flagging.Messages.Requests;
using Initialization.Exceptions;
using Microsoft.Data.Sqlite;

namespace Flagging.DAL
{
    public class DalFlaggingLocal
    {
        private const string
            SELECTS = "roomId, abandonedAt, nJoinedUsersWhenAbandoned ",
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblFlags(" +
                    "id INTEGER PRIMARY KEY," +
                    "userIdFlagging INTEGER NOT NULL," +
                    "userIdBeingFlagged INTEGER NOT NULL," +
                    "flagType INTEGER NOT NULL," +
                    "scopeType INTEGER NOT NULL," +
                    "scopeId INTEGER NOT NULL," +
                    "scopeId2 INTEGER," +
                    "flaggedAt INTEGER NOT NULL," +
                    "description TEXT," +
                    "dealtWithAt INTEGER" +
                ");" +
                "CREATE INDEX IF NOT EXISTS ix_tblFlags_flagType ON tblFlags(flagType);",
            ADD_COMMAND="INSERT INTO tblFlags(" +
                    "userIdFlagging," +
                    "userIdBeingFlagged," +
                    "flagType," +
                    "scopeType," +
                    "scopeId," +
                    "scopeId2," +
                    "flaggedAt," +
                    "description" +
            ") "+
            "values(" +
                "@userIdFlagging, @userIdBeingFlagged," +
                "@flagType, @scopeType, @scopeId, " +
                "@scopeId2, @flaggedAt, @description" +
            ")";
        private static DalFlaggingLocal _Instance;
        public static DalFlaggingLocal Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalFlaggingLocal));
                return _Instance;
            }
        }
        public static DalFlaggingLocal Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalFlaggingLocal));
                _Instance = new DalFlaggingLocal();
                return _Instance;
        }
        private LocalSQLite _LocalSqlite;
        private DalFlaggingLocal() :base(
            )
        {
            _LocalSqlite = new LocalSQLite(
                DependencyManagement.DependencyManager.GetString(
                    DependencyNames.FlaggingDatabaseFilePath),
                    true
                );
            _LocalSqlite.UsingConnectionForWrite((connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    CREATE_COMMAND, connection))
                {
                    command.ExecuteNonQuery();
                }
            });
        }
        public void Append(FlagRequest flagRequest) {
            flagRequest.FlaggedAt = TimeHelper.MillisecondsNow;
            _LocalSqlite.UsingConnectionForWrite((connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    ADD_COMMAND, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@userIdFlagging", flagRequest.UserIdFlagging));
                    command.Parameters.Add(new SqliteParameter("@userIdBeingFlagged", flagRequest.UserIdBeingFlagged));
                    command.Parameters.Add(new SqliteParameter("@flagType", flagRequest.FlagType));
                    command.Parameters.Add(new SqliteParameter("@scopeType", flagRequest.ScopeType));
                    command.Parameters.Add(new SqliteParameter("@scopeId", flagRequest.ScopeId));
                    command.Parameters.Add(new SqliteParameter("@scopeId2", flagRequest.ScopeId2==null?DBNull.Value: flagRequest.ScopeId2));
                    command.Parameters.Add(new SqliteParameter("@flaggedAt", flagRequest.FlaggedAt));
                    command.Parameters.Add(new SqliteParameter("@description", flagRequest.Description==null?DBNull.Value:flagRequest.Description));                    
                    command.ExecuteNonQuery();
                }
            });
        }
    }
}