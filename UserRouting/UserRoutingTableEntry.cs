using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UserRouting
{
    [DataContract]
    public class UserRoutingTableEntry
    {
        private long _Timestamp;
        [JsonPropertyName(UserRoutingTableEntryDataMemberNames.Timestamp)]
        [JsonInclude]
        [DataMember(Name = UserRoutingTableEntryDataMemberNames.Timestamp, EmitDefaultValue = true)]
        public long Timestamp
        {
            get { return _Timestamp; }
            set { _Timestamp = value; } 
        }
        private long _UserId;
        [JsonPropertyName(UserRoutingTableEntryDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserRoutingTableEntryDataMemberNames.UserId, EmitDefaultValue = true)]
        public long UserId {
            get { return _UserId; }
            protected set { _UserId = value; } 
        }
        private List<NodeIdSessionIdPair> _NodeIdSessionIdPairs;
        [JsonPropertyName(UserRoutingTableEntryDataMemberNames.NodeIdDeviceIdentifierPairs)]
        [JsonInclude]
        [DataMember(Name =UserRoutingTableEntryDataMemberNames.NodeIdDeviceIdentifierPairs, EmitDefaultValue = true)]
        public List<NodeIdSessionIdPair> NodeIdSessionIdPairs {
            get
            {
                lock (_NodeIdSessionIdPairs)
                {
                    return _NodeIdSessionIdPairs;
                }
            }
            protected set { _NodeIdSessionIdPairs = value; }
        }
        public long[] GetSessionIdsForNode(int nodeId)
        {
            lock (_NodeIdSessionIdPairs)
            {
                return _NodeIdSessionIdPairs
                    .Where(nodeIdSessionIdPair => nodeIdSessionIdPair.NodeId == nodeId)
                    .Select(nodeIdSessionIdPair => nodeIdSessionIdPair.SessionId)
                    .ToArray();
            }
        }
        public int[] GetNodeIds()
        {
            lock (_NodeIdSessionIdPairs)
            {
                return _NodeIdSessionIdPairs.Select(p => p.NodeId).ToArray();
            }
        }
        public int[] GetOtherNodeIds(int myNodeId)
        {
            lock (_NodeIdSessionIdPairs)
            {
                return _NodeIdSessionIdPairs
                    .Where(p => p.NodeId != myNodeId)
                    .Select(p => p.NodeId)
                    .ToArray();
            }
        }
        public int[] GetOtherNodeIds(int[] excludingNodeIds)
        {
            lock (_NodeIdSessionIdPairs)
            {
                return _NodeIdSessionIdPairs
                    .Where(p => !excludingNodeIds.Contains(p.NodeId))
                    .Select(p => p.NodeId)
                    .ToArray();
            }
        }
        public void AddNodeIdSessionIdPairsIfDoesntHave(UserRoutingTableEntry userRoutingTableEntry) {

            lock (_NodeIdSessionIdPairs)
            {
                foreach (NodeIdSessionIdPair nodeIdSessionIdPair in userRoutingTableEntry.NodeIdSessionIdPairs)
                {
                    if (_NodeIdSessionIdPairs.Where(n => n.NodeId == nodeIdSessionIdPair.NodeId 
                        && n.SessionId == nodeIdSessionIdPair.SessionId).Any()) return;
                    _NodeIdSessionIdPairs.Add(nodeIdSessionIdPair);
                }
            }
        }
        
        public void Add(int nodeId, long sessionId) {
            lock (_NodeIdSessionIdPairs) {
                if (_NodeIdSessionIdPairs.Where(n => n.NodeId == nodeId && n.SessionId == sessionId).Any()) return;
                _NodeIdSessionIdPairs.Add(new NodeIdSessionIdPair(nodeId, sessionId));
            }
        }
        public bool Remove(int nodeId, long sessionId)
        {
            lock (_NodeIdSessionIdPairs)
            {
                _NodeIdSessionIdPairs.RemoveAll(n => (n.NodeId == nodeId) && (n.SessionId == sessionId));
                return _NodeIdSessionIdPairs.Count < 1;
            }
        }
        public UserRoutingTableEntry(long userId, NodeIdSessionIdPair nodeIdSessionIdPair)
        {
            _UserId = userId;
            _NodeIdSessionIdPairs = new List<NodeIdSessionIdPair> { nodeIdSessionIdPair };
        }
        protected UserRoutingTableEntry(long userId)
        {
            _UserId = userId;
            _NodeIdSessionIdPairs = new List<NodeIdSessionIdPair> { };
        }
        protected UserRoutingTableEntry() { }
        public static UserRoutingTableEntry Empty(long userId) {
            return new UserRoutingTableEntry(userId);
        }

    }
}
