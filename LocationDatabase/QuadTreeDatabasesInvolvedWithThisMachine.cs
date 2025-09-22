using Core.Enums;
using Core.Exceptions;
using Location.Interfaces;
using System.Collections.Generic;

namespace LocationDatabase
{
    public static class QuadTreeDatabasesInvolvedWithThisMachine
    {
        private static readonly Dictionary<DatabaseIdentifier, IQuadTreeDatabase> _MapDatabaseIdentifierToDatabase = new Dictionary<DatabaseIdentifier, IQuadTreeDatabase>();
        public static void Register(IQuadTreeDatabase database) {
            DatabaseIdentifier databaseIdentifier = database.Identifier;
            if(_MapDatabaseIdentifierToDatabase.ContainsKey(databaseIdentifier)) {
                throw new DuplicateKeyException(databaseIdentifier);
            }
            _MapDatabaseIdentifierToDatabase[databaseIdentifier] = database;
        }
        public static IQuadTreeDatabase Get(DatabaseIdentifier databaseIdentifier)
        {
            if (!_MapDatabaseIdentifierToDatabase.TryGetValue(databaseIdentifier, out IQuadTreeDatabase database)) {
                throw new KeyNotFoundException($"No key for {nameof(DatabaseIdentifier)} {databaseIdentifier}");
            }
            return database;
        }
    }
}
