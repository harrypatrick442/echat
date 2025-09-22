using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UserRouting
{
    [DataContract]
    public class NodeIdSessionIdPair
    {
        private int _NodeId;
        [JsonPropertyName(NodeIdDeviceIdentifierPairDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = NodeIdDeviceIdentifierPairDataMemberNames.NodeId, EmitDefaultValue = true)]
        public int NodeId { get { return _NodeId; } protected set { _NodeId = value; } }
        private long _SessionId;
        [JsonPropertyName(NodeIdDeviceIdentifierPairDataMemberNames.SessionId)]
        [JsonInclude]
        [DataMember(Name = NodeIdDeviceIdentifierPairDataMemberNames.SessionId, EmitDefaultValue = true)]
        public long SessionId { get { return _SessionId; } protected set { _SessionId = value; } }
        public NodeIdSessionIdPair(int nodeId, long sessionId) {
            _NodeId = nodeId;
            _SessionId = sessionId;
        }
        protected NodeIdSessionIdPair() { }
    }
}
