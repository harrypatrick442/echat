using Core.Exceptions;
using Chat;
using Chat.Interfaces;
using SQLite;
using Database;
using DependencyManagement;
using Microsoft.Data.Sqlite;
namespace Core.DAL
{
    public class DalAbandonedRooms
    {
        private const int MAX_N_CONNECTIONS = 4;
        private const string
            SELECTS = "roomId, abandonedAt, nJoinedUsersWhenAbandoned ",
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblAbandonedRooms(" +
                    "roomId INTEGER PRIMARY KEY," +
                    "abandonedAt INTEGER NOT NULL," +
                    "nJoinedUsersWhenAbandoned INTEGER NOT NULL" +
                ");" +
                "CREATE INDEX IF NOT EXISTS ix_tblAbandonedRooms_abandonedAt ON tblAbandonedRooms(abandonedAt);" +
                "CREATE INDEX IF NOT EXISTS ix_tblAbandonedRooms_nJoinedUsersWhenAbandoned ON tblAbandonedRooms(nJoinedUsersWhenAbandoned);",
            READ_N_MOST_RECENTLY_ABANDONED = SELECTS + "FROM tblAbandonedRooms ORDER BY abandonedAt DESC LIMIT @nEntries;",
            INSERT_OR_REPLACE_COMMAND = "INSERT OR REPLACE INTO tblAbandonedRooms(" +
                "roomId, abandonedAt, nJoinedUsersWhenAbandoned) " +
                "VALUES(@roomId, @abandonedAt, @nJoinedUsersWhenAbandoned);";
        private static DalAbandonedRooms _Instance;
        public static DalAbandonedRooms Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalAbandonedRooms));
                return _Instance;
            }
        }
        public static DalAbandonedRooms Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalAbandonedRooms));
            _Instance = new DalAbandonedRooms();
            return _Instance;
        }
        private LocalSQLite _LocalSQLite;
        private DalAbandonedRooms() {
            _LocalSQLite = new LocalSQLite(
                DependencyManager.GetString(DependencyNames.AbandonedRoomsDatabaseFilePath),
                false);
            _LocalSQLite.UsingConnectionForWrite((connection) => {
                using (SqliteCommand command = new SqliteCommand(
                    CREATE_COMMAND, connection))
                {
                    command.ExecuteNonQuery();
                }
            });
        }
        public void Add(long roomId, long abandonedAt, long nJoinedUsersWhenAbandoned) {
            _LocalSQLite.UsingConnectionForWrite((connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        INSERT_OR_REPLACE_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@roomId", roomId));
                        command.Parameters.Add(new SqliteParameter("@abandonedAt", abandonedAt));
                        command.Parameters.Add(new SqliteParameter("@nJoinedUsersWhenAbandoned", nJoinedUsersWhenAbandoned));
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            });
        }
    }
}