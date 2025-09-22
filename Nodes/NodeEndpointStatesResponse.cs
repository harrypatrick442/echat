using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nodes
{
    [DataContract]
    public class NodeEndpointStatesResponse
    {
        [JsonPropertyName(NodeEndpointStatesResponseDataMemberNames.Live)]
        [JsonInclude]
        [DataMember(Name =NodeEndpointStatesResponseDataMemberNames.Live)]
        public NodeEndpointState[] Live { get; protected set; }
        [JsonPropertyName(NodeEndpointStatesResponseDataMemberNames.History)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStatesResponseDataMemberNames.History)]
        public NodeEndpointState[] History { get; protected set; }
        public NodeEndpointStatesResponse(NodeEndpointState[] live, 
            NodeEndpointState[] history) {
            Live = live;
            History = history;
        }
        protected NodeEndpointStatesResponse() { }
    }
}
