
using InterserverComs;

namespace KeyValuePairDatabases
{
    public interface IIdentifiedKeyValuePairDatabaseMesh
    {
        int DatabaseIdentifier { get; }
        void HandleMessageForThisParticularDatabase(
            InterserverMessageEventArgs e, RemoteOperationRequest remoteOperationRequest);
    }
}