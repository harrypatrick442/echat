using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Enums;
using MessageTypes.Internal;
using Location.DataMemberNames.Interserver.Requests;

namespace Location.Requests
{
    [DataContract]
    public class GetNEntriesSpecificToNodeRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetNEntriesSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = GetNEntriesSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        public DatabaseIdentifier DatabaseIdentifier { get; protected set; }
        [JsonPropertyName(GetNEntriesSpecificToNodeRequestDataMemberNames.Level)]
        [JsonInclude]
        [DataMember(Name = GetNEntriesSpecificToNodeRequestDataMemberNames.Level)]
        public int Level { get; protected set; }
        [JsonPropertyName(GetNEntriesSpecificToNodeRequestDataMemberNames.Quadrants)]
        [JsonInclude]
        [DataMember(Name = GetNEntriesSpecificToNodeRequestDataMemberNames.Quadrants)]
        public long[] Quadrants { get; protected set; }
        [JsonPropertyName(GetNEntriesSpecificToNodeRequestDataMemberNames.WithLatLng)]
        [JsonInclude]
        [DataMember(Name = GetNEntriesSpecificToNodeRequestDataMemberNames.WithLatLng)]
        public bool WithLatLng { get; protected set; }
        public GetNEntriesSpecificToNodeRequest(DatabaseIdentifier databaseIdentifier,
            int level, long[] quadrants) : 
            base(InterserverMessageTypes.QuadTreeGetIdsSpecificToNode)
        {
            DatabaseIdentifier = databaseIdentifier;
            Level = level;
            Quadrants = quadrants;
        }
        protected GetNEntriesSpecificToNodeRequest() :
            base(InterserverMessageTypes.QuadTreeGetIdsSpecificToNode)
        { }
    }
}
