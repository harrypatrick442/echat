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
    public class SetOnIdAssociatedNodeRequest : TicketedMessageBase
    {
        [JsonPropertyName(SetOnIdAssociatedNodeRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = SetOnIdAssociatedNodeRequestDataMemberNames.DatabaseIdentifier)]
        public DatabaseIdentifier DatabaseIdentifier { get; protected set; }
        [JsonPropertyName(SetOnIdAssociatedNodeRequestDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = SetOnIdAssociatedNodeRequestDataMemberNames.Id)]
        public long Id { get; protected set; }
        [JsonPropertyName(SetOnIdAssociatedNodeRequestDataMemberNames.LatLng)]
        [JsonInclude]
        [DataMember(Name = SetOnIdAssociatedNodeRequestDataMemberNames.LatLng)]
        public LatLng LatLng { get; protected set; }
        public SetOnIdAssociatedNodeRequest(DatabaseIdentifier databaseIdentifier, long id, LatLng latLng) : 
            base(InterserverMessageTypes.QuadTreeSetOnIdAssociatedNode)
        {
            DatabaseIdentifier = databaseIdentifier;
            Id = id;
            LatLng = latLng;
        }
        protected SetOnIdAssociatedNodeRequest() :
            base(InterserverMessageTypes.QuadTreeSetOnIdAssociatedNode)
        { }
    }
}
