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
    public class SetSpecificToNodeRequest : TicketedMessageBase
    {
        [JsonPropertyName(SetSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = SetSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        public DatabaseIdentifier DatabaseIdentifier { get; protected set; }
        [JsonPropertyName(SetSpecificToNodeRequestDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = SetSpecificToNodeRequestDataMemberNames.Id)]
        public long Id { get; protected set; }
        [JsonPropertyName(SetSpecificToNodeRequestDataMemberNames.LevelQuadrantPairs)]
        [JsonInclude]
        [DataMember(Name = SetSpecificToNodeRequestDataMemberNames.LevelQuadrantPairs)]
        public LevelQuadrantPair[] LevelQuadrantPairs { get; protected set; }
        [JsonPropertyName(SetSpecificToNodeRequestDataMemberNames.LatLng)]
        [JsonInclude]
        [DataMember(Name = SetSpecificToNodeRequestDataMemberNames.LatLng)]
        public LatLng LatLng { get; protected set; }
        public SetSpecificToNodeRequest(DatabaseIdentifier databaseIdentifier, long id, LatLng latLng, LevelQuadrantPair[] levelQuadrantPairs) : 
            base(InterserverMessageTypes.QuadTreeSetSpecificToNode)
        {
            DatabaseIdentifier = databaseIdentifier;
            Id = id;
            LatLng = latLng;
            LevelQuadrantPairs = levelQuadrantPairs;
        }
        protected SetSpecificToNodeRequest() :
            base(InterserverMessageTypes.QuadTreeSetSpecificToNode)
        { }
    }
}
