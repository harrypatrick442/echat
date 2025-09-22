using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Enums;
using LocationCore;
using MessageTypes.Internal;
using Location.DataMemberNames.Interserver.Requests;

namespace Location.Requests
{
    [DataContract]
    public class GetIdsSpecificToNodeRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetIdsSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = GetIdsSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        public DatabaseIdentifier DatabaseIdentifier { get; protected set; }
        [JsonPropertyName(GetIdsSpecificToNodeRequestDataMemberNames.LevelQuadrantPairs)]
        [JsonInclude]
        [DataMember(Name = GetIdsSpecificToNodeRequestDataMemberNames.LevelQuadrantPairs)]
        public LevelQuadrantPair[] LevelQuadrantPairs { get; protected set; }
        public GetIdsSpecificToNodeRequest(DatabaseIdentifier databaseIdentifier,
            LevelQuadrantPair[] levelQuadrantPairs) : 
            base(InterserverMessageTypes.QuadTreeGetIdsSpecificToNode)
        {
            DatabaseIdentifier = databaseIdentifier;
            LevelQuadrantPairs = levelQuadrantPairs;
        }
        protected GetIdsSpecificToNodeRequest() :
            base(InterserverMessageTypes.QuadTreeGetIdsSpecificToNode)
        { }
    }
}
