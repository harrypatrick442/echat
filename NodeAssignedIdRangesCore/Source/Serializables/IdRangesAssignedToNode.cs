using KeyValuePairDatabases;
using NodeAssignedIdRanges;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesSource.Serializables
{
    [DataContract]
    public class IdRangesAssignedToNode
    {
        //CHECKED
        [JsonPropertyName(IdRangesAssignedToNodeDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = IdRangesAssignedToNodeDataMemberNames.NodeId)]
        public int NodeId { get; set; }
        private List<IdRange> _NodeIdRanges;
        [JsonPropertyName(IdRangesAssignedToNodeDataMemberNames.IdRanges)]
        [JsonInclude]
        [DataMember(Name = IdRangesAssignedToNodeDataMemberNames.IdRanges)]
        public IdRange[] IdRanges
        {
            get { lock (this) { return _NodeIdRanges.ToArray(); } }
            protected set { _NodeIdRanges = value.ToList(); }
        }
        public void Add(IdRange range)
        {
            lock (this)
            {
                _NodeIdRanges.Add(range);
            }
        }

        public IdRangesAssignedToNode()
        {

        }
            public IdRangesAssignedToNode(int nodeId, IdRange firstRange)
        {
            _NodeIdRanges = new List<IdRange> { firstRange };
            NodeId = nodeId;
        }
    }
}
