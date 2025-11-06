using Flagging.DAL;

namespace Flagging
{
    public static class Initializer
    {
        public static void Initialize(bool isFlaggingClient)
        {
            int flaggingNodeId, flaggingBackupNodeId;
#if DEBUG
            flaggingNodeId = Configurations.Nodes.FLAGGING_NODE_ID_DEBUG;
            flaggingBackupNodeId = Configurations.Nodes.FLAGGING_BACKUP_NODE_ID_DEBUG;
#else
            flaggingNodeId = Configurations.Nodes.FLAGGING_NODE_ID;
            flaggingBackupNodeId = Configurations.Nodes.FLAGGING_BACKUP_NODE_ID;
#endif
            int myNodeId = Nodes.Nodes.Instance.MyId;
            bool iAmFlaggingNode = flaggingNodeId == myNodeId||flaggingBackupNodeId==myNodeId;
            if (iAmFlaggingNode)
            {
                DalFlaggingLocal.Initialize();
            }
            if (isFlaggingClient || iAmFlaggingNode)
            {
                FlaggingMesh.Initialize(flaggingNodeId, flaggingBackupNodeId);
            }
        }
    }
}