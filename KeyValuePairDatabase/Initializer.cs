

using KeyValuePairDatabases.Appended;

namespace KeyValuePairDatabases
{
    public static class Initializer
    {
        public static void Initialize()
        {
            KeyValuePairDatabaseIncomingMessagesHandler.Initialize();
            AppendedKeyValuePairDatabaseIncomingMessagesHandler.Initialize();
        }
    }
}