using Core.Enums;
using Core.Exceptions;
using Users;
using KeyValuePairDatabases;
using Microsoft.Data.Sqlite;
using Database;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.Enums;
using System.Data;
using DependencyManagement;
using Initialization.Exceptions;
namespace Users.DAL
{
    public class DalUserProfiles
    {
        private static DalUserProfiles _Instance;
        private LocalSQLite _UsernameSearchSqliteLocalDatabase;
        public static DalUserProfiles Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalUserProfiles));
                return _Instance;
            }
        }
        public static DalUserProfiles Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalUserProfiles));
                _Instance = new DalUserProfiles();
                return _Instance;
        }

        private KeyValuePairDatabaseMesh<long, UserProfile> _UserIdToUserProfileKeyValuePairDatabase;
        private DalUserProfiles()
        {
            _UserIdToUserProfileKeyValuePairDatabase
            = new KeyValuePairDatabaseMesh<long, UserProfile>(
                DatabaseIdentifier.UserIdToUserProfile.Int(), 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToUserProfileDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension = ".json"
                }, new IdentifierLock<long>(), UserIdToNodeId.Instance);
            _UsernameSearchSqliteLocalDatabase = new LocalSQLite(
                DependencyManager.GetString(DependencyNames.UsernameSearchSqliteLocalDatabaseFilePath), useUTF16:true);
            CreateTblUsernameSearch();
        }
        public UserProfile GetUserProfile(long userId)
        {
            UserProfile userProfile =  _UserIdToUserProfileKeyValuePairDatabase.Get(userId);
            return userProfile;
        }
        public void ModifyUserProfile(long userId, Func<UserProfile, UserProfile> callback)
        {
            _UserIdToUserProfileKeyValuePairDatabase.ModifyWithinLock(userId, callback);
        }
        public void CreateTblUsernameSearch()
        {
            _UsernameSearchSqliteLocalDatabase.UsingConnectionForWrite((connection) =>
            {
                using (SqliteCommand command = new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS tblUsernamesSearch (userId INTEGER PRIMARY KEY, username TEXT);"
                    + " CREATE INDEX IF NOT EXISTS indexUsername ON tblUsernamesSearch (username);",
                    connection)){
                    command.ExecuteNonQuery();
                }
            });
        }
        public void UsernameSearchAddUser(long userId, string username) {
            _UsernameSearchSqliteLocalDatabase.UsingConnectionForWrite((connection) =>
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (SqliteCommand command = new SqliteCommand(
                        "INSERT OR REPLACE INTO tblUsernamesSearch (userId, username) VALUES(@userId,@username);",
                        connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@userId", userId));
                        command.Parameters.Add(new SqliteParameter("@username", username));
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            });
        }
        public void UsernameSearchRemoveUser(long userId)
        {
            _UsernameSearchSqliteLocalDatabase.UsingConnectionForWrite((connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        "DELETE FROM tblUsernamesSearch WHERE and userId = @userId;",
                        connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@userId", userId));
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            });
        }
        public long[] UsernameSearchSearch(string str, int maxNEntries)
        {
            return _UsernameSearchSqliteLocalDatabase.UsingConnection((connection) =>
            {
                str = EscapeLike(str);
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        "SELECT userId FROM tblUsernamesSearch WHERE username LIKE @str  ESCAPE '\\' LIMIT @maxNEntries;",
                        connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@str", $"%{str}%"));
                        command.Parameters.Add(new SqliteParameter("@maxNEntries", maxNEntries));
                        using (SqliteDataReader dataReader = command.ExecuteReader())
                        {
                            List<long> userIds = new List<long>();
                            while (dataReader.Read())
                            {
                                userIds.Add(dataReader.GetInt64(0));
                            }
                            return userIds.ToArray();
                        }
                    }
                }
            });
        }
        private string EscapeLike(string str)
        {
            if (str == null) return null;
            StringBuilder escaped = new StringBuilder();
            foreach (char c in str)
            {
                if (c == '%')
                {
                    escaped.Append("\\%");
                    continue;
                }
                if (c == '_')
                {
                    escaped.Append("\\_");
                    continue;
                }
                if (c == '\\')
                {
                    escaped.Append("\\\\");
                    continue;
                }
                escaped.Append(c);
            }
            return escaped.ToString();
        }
    }
}