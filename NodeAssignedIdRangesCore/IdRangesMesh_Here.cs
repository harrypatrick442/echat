using ConfigurationCore;
using DependencyManagement;
using NodeAssignedIdRangesCore.Requests;

namespace NodeAssignedIdRanges
{
    public sealed partial class IdRangesMesh
    {
        private INodesConfiguration? _NodesConfiguration;
        private INodesConfiguration NodesConfiguration { 
            get 
            {
                if (_NodesConfiguration == null) {
                    _NodesConfiguration = DependencyManager.Get<INodesConfiguration>();
                }
                return _NodesConfiguration;
            } 
        }
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
            int[] associatedIdTypes = NodesConfiguration.GetAssociatedIdTypes(nodeId);
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