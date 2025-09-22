using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Timing;

namespace Nodes
{
    [DataContract]
    public class NodeEndpointState
    {
        [JsonPropertyName(NodeEndpointStateDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.NodeId)]
        public int NodeId { get; protected set; }
        [JsonPropertyName(NodeEndpointStateDataMemberNames.IsOpen)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.IsOpen)]
        public bool IsOpen { get; set; }
        [JsonPropertyName(NodeEndpointStateDataMemberNames.InstanceId)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.InstanceId)]
        public long InstanceId { get; protected set; }
        [JsonPropertyName(NodeEndpointStateDataMemberNames.OpenedAt)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.OpenedAt)]
        public long OpenedAt { get; protected set; }
        [JsonPropertyName(NodeEndpointStateDataMemberNames.IAmClient)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.IAmClient)]
        public bool IAmClient { get; protected set; }
        [JsonPropertyName(NodeEndpointStateDataMemberNames.ClosedAt)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.ClosedAt)]
        public long ClosedAt { get; protected set; }
        [JsonPropertyName(NodeEndpointStateDataMemberNames.ConnectedTimestamp)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.ConnectedTimestamp)]
        public long ConnectedTimestamp { get; protected set; }
        private List<long> _CloseEventsAt = new List<long>();
        [JsonPropertyName(NodeEndpointStateDataMemberNames.CloseEventsAt)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.CloseEventsAt)]
        public long[] CloseEventsAt { get { return _CloseEventsAt.ToArray(); } protected set { } }
        private List<long> _OpenEventsAt = new List<long>();
        [JsonPropertyName(NodeEndpointStateDataMemberNames.OpenEventsAt)]
        [JsonInclude]
        [DataMember(Name = NodeEndpointStateDataMemberNames.OpenEventsAt)]
        public long[] OpenEventsAt { get { return _OpenEventsAt.ToArray(); } protected set { } }
        public void Opened()
        {
            lock (this)
            {
                long now = TimeHelper.MillisecondsNow;
                IsOpen = true;
                OpenedAt = now;
            }
        }
        public void OpenEvent()
        {
            lock (this)
            {
                long now = TimeHelper.MillisecondsNow;
                _OpenEventsAt.Add(now);
            }
        }
        public void Closed()
        {
            lock (this)
            {
                long now = TimeHelper.MillisecondsNow;
                IsOpen = false;
                ClosedAt = now;
            }
        }
        public void CloseEvent()
        {
            lock (this)
            {
                long now = TimeHelper.MillisecondsNow;
                _CloseEventsAt.Add(now);
            }
        }
        public NodeEndpointState(int nodeId, long instanceId, long connectedTimestamp, bool iAmClient) {
            NodeId = nodeId;
            ConnectedTimestamp = connectedTimestamp;
            InstanceId = instanceId;
            IAmClient = iAmClient;
        }
        protected NodeEndpointState() { }
        public NodeEndpointState Clone() {
            lock (this)
            {
                return new NodeEndpointState(NodeId, InstanceId, ConnectedTimestamp, IAmClient)
                {
                    ClosedAt = this.ClosedAt,
                    OpenedAt = this.OpenedAt,
                    IsOpen = this.IsOpen
                };
            }
        }

    }
}
