using NodeAssignedIdRangesCore.Requests;

namespace NodeAssignedIdRanges
{
    public sealed partial class IdRangesMesh
    {
        //CHECKED
        private IdRange GiveMeNewIdRange_Here(int idType, int nodeId)
        {
            IdRange newIdRange = SourceIdRangesManager.Instance.ForIdType(idType)
                .AssignNextIdRangeToNode(nodeId);
            SendAnotherServerGotANewIdRangeToOtherNodesConnected(
                idType, nodeIdAssignedTo: nodeId, newIdRange);
            return newIdRange;
        }
        public void AnotherServerGotANewIdRange_Here(int idType, int nodeId, IdRange range)
        {
            NodesIdRangesManager.Instance.AnotherNodeGotNewIdRange(idType, nodeId, range);
        }
        public NodesIdRangesForIdType[] GetNodesIdRangesForAllAssociatedIdTypes_Here(int nodeId)
        {
            int[] associatedIdTypes = GlobalConstants.Nodes.GetAssociatedIdTypes(nodeId);
            List<NodesIdRangesForIdType> list = new List<NodesIdRangesForIdType>();
            foreach (int idType in associatedIdTypes)
            {
                NodeIdRanges[] nodeIdRangess = SourceIdRangesManager.Instance
                    .ForIdType(idType)
                    .GetIdRangesAssignedToNodes();
                list.Add(new NodesIdRangesForIdType(idType, nodeIdRangess));
            }
            return list.ToArray();
        }
    }
}