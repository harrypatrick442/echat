using Core.Delegates;
using Database;
using Microsoft.Data.Sqlite;
using System.Data;

namespace KeyValuePairDatabases.OnDiskDatabases
{
    //https://www.reddit.com/r/golang/comments/16xswxd/concurrency_when_writing_data_into_sqlite/
    //https://www.sqlite.org/lang_transaction.html
    //https://www.sqlite.org/cgi/src/doc/begin-concurrent/doc/begin_concurrent.md
    //https://sqlite.org/hctree/doc/hctree/doc/hctree/index.html
    //https://www.sqlite.org/lang_transaction.html#:~:text=A%20read%20transaction%20is%20used,collectively%20%22write%20statements%22).
    //best way to do locking:
    //https://www3.sqlite.org/cgi/src/doc/begin-concurrent/doc/begin_concurrent.md
    //https://phiresky.github.io/blog/2020/sqlite-performance-tuning/
    //bit outdated since wal2 now exists.
    //https://www.reddit.com/r/golang/comments/16xswxd/concurrency_when_writing_data_into_sqlite/
    //https://www.sqlite.org/src/vdiff?branch=begin-concurrent
/*
 * If there is significant contention for the writer lock, this mechanism can be
inefficient. In this case it is better for the application to use a mutex or
some other mechanism that supports blocking to ensure that at most one writer
is attempting to COMMIT a BEGIN CONCURRENT transaction at a time. This is
usually easier if all writers are part of the same operating system process.
*/
//THIS IS ONE EXAMPLE OF WHERE A LOCK OR MUTEX IS BETTER.
//dont need commits on selects. remove them.
public class KeyValuePairOnDiskDatabaseSqliteStrings<TIdentifier>
{
    private IIdentifierLock<TIdentifier> _IdentifierBasedFileLock;
    private LocalSQLite _LocalSQLite;
    private static readonly Type[] TYPES_THAT_MAP_TO_SQLITE_INTEGER = new Type[] { typeof(int), typeof(long)};
    public KeyValuePairOnDiskDatabaseSqliteStrings(string rootDirectory, string filePath, 
            IIdentifierLock<TIdentifier> identifierLock, int? stringKeyLength = null)
    {
        _IdentifierBasedFileLock = identifierLock;
        if (filePath == null)
        {
            if (rootDirectory == null)
                throw new ArgumentException($"{nameof(rootDirectory)} was null");
            filePath = Path.Combine(rootDirectory, $"kvp.sqlite");
        }
        else {
            string extension = Path.GetExtension(filePath);
            if (extension == "") {
                filePath += ".sqlite";
            }
            else
            {
                if (extension != ".sqlite")
                    throw new ArgumentException($"Unsupported extension {extension}");
            }
        }
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        _LocalSQLite = new LocalSQLite(filePath, false);
        Type identifierType = typeof(TIdentifier);
        string keyType;
        if (identifierType.Equals(typeof(string)))
        {/*
            if (stringKeyLength == null)
                throw new ArgumentException($"{nameof(stringKeyLength)} not provided");
            if (stringKeyLength < 1)
                throw new ArgumentException($"{nameof(stringKeyLength)} cannot be {stringKeyLength}");
                */
                keyType = $"TEXT";// $"VARCHAR({(int)stringKeyLength})";
        }
        else if (TYPES_THAT_MAP_TO_SQLITE_INTEGER.Contains(identifierType)) {
            keyType = "INTEGER";
        }
        else
        {
            throw new ArgumentException($"{nameof(TIdentifier)} not supported as a key");
        }
        _LocalSQLite.UsingConnectionForWrite((connection) =>
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (SqliteCommand command = new SqliteCommand(
                    $"CREATE TABLE IF NOT EXISTS tblKvp (key {keyType} PRIMARY KEY, value TEXT);",
                    connection, transaction))
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        });
    }

    public void Delete(TIdentifier key)
    {
        _IdentifierBasedFileLock.LockForWrite(key, () =>
        {
            _LocalSQLite.UsingConnectionForWrite((connection) =>
            {
                _Delete(connection, key);
            });
        });
    }

    public bool Has(TIdentifier key)
    {
        return _IdentifierBasedFileLock.LockForReads(key, () =>
        {
            return _LocalSQLite.UsingConnection((connection) =>
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqliteCommand command = new SqliteCommand(
                        "SELECT EXISTS(SELECT 1 FROM tblKvp WHERE key=@key);",
                        connection, transaction))
                    {
                        command.Parameters.Add(new SqliteParameter("@key", key));
                        bool exists = (bool)command.ExecuteScalar();
                        return exists;
                    }
                }
            });
        });
    }

    public void IterateEntries(Action<DelegateNextEntry<string>> callback)
    {
        throw new NotImplementedException();
    }

    public string Read(TIdentifier key)
    {
        return _IdentifierBasedFileLock.LockForReads(key, () =>
        {
            return _LocalSQLite.UsingConnection((connection) =>
            {
                return _Read(connection, key);
            });
        });
    }

    public void ReadCallbackDeleteWithinLock(TIdentifier key, Action<string> callback)
    {
        _IdentifierBasedFileLock.LockForWrite(key, () =>
        {
            string value = _LocalSQLite.UsingConnection((connection) =>
                _Read(connection, key)
            );
            callback(value); 
            _LocalSQLite.UsingConnectionForWrite((connection) =>
                _Delete(connection, key)
            );
        });
    }

    public void ReadCallbackWriteWithinLock(TIdentifier key, Func<string, string> callback)
    {
            _IdentifierBasedFileLock.LockForWrite(key, () =>
            {
                string value = _LocalSQLite.UsingConnection((connection) =>
                    _Read(connection, key)
                );
                value = callback(value);
                _LocalSQLite.UsingConnectionForWrite((connection) =>
                    _Write(connection, key, value)
                );
        });
    }

    public string ReadWithoutLock(TIdentifier key)
    {
        return _LocalSQLite.UsingConnection((connection) =>
            _Read(connection, key)
        );
    }
    public void Write(TIdentifier key, string value)
    {
        _IdentifierBasedFileLock.LockForWrite(key, () =>
        {
            _LocalSQLite.UsingConnectionForWrite((connection) =>
                _Write(connection, key, value)
            );
        });
    }
    private string _Read(SqliteConnection connection, TIdentifier key)
    {
        using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
        {
            using (SqliteCommand command = new SqliteCommand(
            "SELECT value FROM tblKvp WHERE key=@key;",
            connection, transaction))
            {
                command.Parameters.Add(new SqliteParameter("@key", key));
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.IsDBNull(0)?null:reader.GetString(0);
                    }
                    return null;
                }
            }
        }
    }
    private void _Delete(SqliteConnection connection, TIdentifier key)
    {
        using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
        {
            using (SqliteCommand command = new SqliteCommand(
            "DELETE FROM tblKvp WHERE key = @key;",
            connection, transaction))
            {
                command.Parameters.Add(new SqliteParameter("@key", key));
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
    private void _Write(SqliteConnection connection, TIdentifier key, string value) {

        using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
        {
            using (SqliteCommand command = new SqliteCommand(
            "INSERT OR REPLACE INTO tblKvp(key,value) VALUES(@key,@value);",
            connection, transaction))
            {
                command.Parameters.Add(new SqliteParameter("@key", key));
                command.Parameters.Add(new SqliteParameter("@value", value==null?DBNull.Value:value));
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
}
}