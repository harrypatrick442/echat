
using InterserverComs;
using KeyValuePairDatabases.Appended;

namespace KeyValuePairDatabases
{
    public interface IAppendedKeyValuePairDatabaseIncomingMessagesHandler
    {
        int DatabaseIdentifier { get; }
        void HandleAppendedRead(InterserverMessageEventArgs e, AppendedReadRequest request);
        void HandleAppendedAppend(InterserverMessageEventArgs e, AppendedAppendRequest request);
    }
}