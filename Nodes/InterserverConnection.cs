using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nodes
{
    [DataContract]
    public class InterserverConnection
    {
        [JsonPropertyName(InterserverConnectionDataMemberNames.IAmClientElseServer)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionDataMemberNames.IAmClientElseServer)]
        public bool IAmClientElseServer { get; protected set; }

        [JsonPropertyName(InterserverConnectionDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionDataMemberNames.NodeId)]
        public int NodeId { get; protected set; }

        [JsonPropertyName(InterserverConnectionDataMemberNames.ServerUrl)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionDataMemberNames.ServerUrl)]
        public string ServerUrl { get; protected set; }
        [JsonPropertyName(InterserverConnectionDataMemberNames.Hash)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionDataMemberNames.Hash)]
        public string Hash { get; protected set; }
        [JsonPropertyName(InterserverConnectionDataMemberNames.Password)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionDataMemberNames.Password)]
        public string Password { get; protected set; }
        [JsonPropertyName(InterserverConnectionDataMemberNames.ExpectedIPAddressOfClient)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionDataMemberNames.ExpectedIPAddressOfClient)]
        public string ExpectedIPAddressOfClient { get; protected set; }
        public InterserverConnection(int nodeId, string hash, string password, string serverUrl, 
            bool iAmClientElseServer, string expectedIPAddressOfClient) { 
            NodeId = nodeId;   
            Hash = hash;
            Password = password;
            ServerUrl = serverUrl;
            IAmClientElseServer=iAmClientElseServer;
            ExpectedIPAddressOfClient = expectedIPAddressOfClient;
        }
        protected InterserverConnection() { }
    }
}
