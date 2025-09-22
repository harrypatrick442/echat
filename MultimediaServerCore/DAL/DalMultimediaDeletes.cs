using Database;
using Microsoft.Data.Sqlite;
using Core.Timing;
using System.Text;
using Core.Exceptions;
using DependencyManagement;
namespace MultimediaServerCore
{
    internal class DalMultimediaDeletes
    {
        private static DalMultimediaDeletes? _Instance;
        public static DalMultimediaDeletes Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMultimediaDeletes));
                return _Instance;
            }
        }
        public static DalMultimediaDeletes Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMultimediaDeletes));
            _Instance = new DalMultimediaDeletes();
            return _Instance;
        }
        private const int MAX_N_CONNECTIONS = 4;
        private const string
            COLUMNS = "id INTEGER PRIMARY KEY, filePath TEXT NOT NULL, scheduledAt INTEGER NOT NULL",
            CREATE_COMMAND =
                "CREATE TABLE IF NOT EXISTS tblPendingDeletes(" +COLUMNS+");" +
                "CREATE INDEX IF NOT EXISTS ix_tblPendingDeletes_id ON tblPendingDeletes(id);" +
                "CREATE INDEX IF NOT EXISTS ix_tblPendingDeletes_scheduledAt ON tblPendingDeletes(scheduledAt);" +
                "CREATE TABLE IF NOT EXISTS tblFailedDeletes("+COLUMNS+");" +
                "CREATE INDEX IF NOT EXISTS ix_tblFailedDeletes_id ON tblFailedDeletes(id);" +
                "CREATE INDEX IF NOT EXISTS ix_tblFailedDeletes_scheduledAt ON tblFailedDeletes(scheduledAt);",
            ADD_PENDING_COMMAND = "INSERT INTO tblPendingDeletes(filePath,scheduledAt) VALUES(@filePath,@scheduledAt);",
            ADD_FAILED_COMMAND = "INSERT INTO tblFailedDeletes(filePath,scheduledAt) VALUES(@filePath,@scheduledAt);",
            DELETE_PENDING_COMMAND = "DELETE FROM tblPendingDeletes WHERE id = @id;",
            DELETE_FAILED_COMMAND = "DELETE FROM tblFailedDeletes WHERE id = @id;",
            GET_PENDINGS_COMMAND= "SELECT id,filePath,scheduledAt FROM tblPendingDeletes WHERE" +
            " scheduledAt<@maxscheduledAt ORDER BY scheduledAt LIMIT @maxNEntries;";
        private LocalSQLite _LocalSQLite;
        private DalMultimediaDeletes()
        {
            _LocalSQLite = new LocalSQLite(DependencyManager.GetString(DependencyNames.MultimediaDeletesDatabaseFilePath), false, 1);
            _LocalSQLite.UsingConnectionForWrite(Prepare);
        }
        private void Prepare(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (SqliteCommand command = new SqliteCommand(
                    CREATE_COMMAND, connection, transaction))
                {
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }
        public void AddPending(PendingMultimediaDelete pendingMultimediaDelete)
        {
            _LocalSQLite.UsingConnectionForWrite((connection) => {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        ADD_PENDING_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@filePath", pendingMultimediaDelete.FilePath));
                        command.Parameters.Add(new SqliteParameter("@scheduledAt", pendingMultimediaDelete.ScheduledAt));
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            });
        }
        public void AddFailed(PendingMultimediaDelete pendingMultimediaDelete)
        {
            _LocalSQLite.UsingConnectionForWrite((connection) => {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        ADD_FAILED_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@filePath", pendingMultimediaDelete.FilePath));
                        command.Parameters.Add(new SqliteParameter("@scheduledAt", pendingMultimediaDelete.ScheduledAt));
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            });
        }
        public void DeletePending(long id)
        {
            _LocalSQLite.UsingConnectionForWrite((connection) => {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        DELETE_PENDING_COMMAND, connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@id", id));
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            });
        }
        public PendingMultimediaDelete[] GetNextPendings(int maxNEntries)
        {
            List<PendingMultimediaDelete> entries = new List<PendingMultimediaDelete>();
            _LocalSQLite.UsingConnection((connection) =>
            {
                StringBuilder sbCommand = new StringBuilder();
                long maxscheduledAt = TimeHelper.MillisecondsNow - GlobalConstants.Delays.MIN_TIME_MILLISECONDS_BEFORE_MUTIMEDIA_DELETE_FILE_FROM_SCHEDULED;
                using (SqliteCommand command = new SqliteCommand(
                    GET_PENDINGS_COMMAND, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries));
                    command.Parameters.Add(new SqliteParameter("@maxscheduledAt", maxscheduledAt));
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            entries.Add(new PendingMultimediaDelete(reader.GetInt64(0), reader.GetString(1), reader.GetInt64(2)));
                        }
                    }
                }
            });
            return entries.ToArray();
        }
    }
}