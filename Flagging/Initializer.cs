using Flagging.DAL;

namespace Flagging
{
    public static class Initializer
    {
        public static void Initialize(bool isFlaggingClient)
        {
            int flaggingNodeId, flaggingBackupNodeId;
#if DEBUG
            flaggingNodeId = GlobalConstants.Nodes.FLAGGING_NODE_ID_DEBUG;
            flaggingBackupNodeId = GlobalConstants.Nodes.FLAGGING_BACKUP_NODE_ID_DEBUG;
#else
            flaggingNodeId =GlobalConstants.Nodes.FLAGGING_NODE_ID;
            flaggingBackupNodeId =GlobalConstants.Nodes.FLAGGING_BACKUP_NODE_ID;
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