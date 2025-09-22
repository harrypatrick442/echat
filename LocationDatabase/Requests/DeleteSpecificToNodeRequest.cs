using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Enums;
using MessageTypes.Internal;
using Location.DataMemberNames.Interserver.Requests;

namespace Location.Requests
{
    [DataContract]
    public class DeleteSpecificToNodeRequest : TicketedMessageBase
    {
        [JsonPropertyName(DeleteSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = DeleteSpecificToNodeRequestDataMemberNames.DatabaseIdentifier)]
        public DatabaseIdentifier DatabaseIdentifier { get; protected set; }
        [JsonPropertyName(DeleteSpecificToNodeRequestDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = DeleteSpecificToNodeRequestDataMemberNames.Id)]
        public long Id { get; protected set; }
        [JsonPropertyName(DeleteSpecificToNodeRequestDataMemberNames.Levels)]
        [JsonInclude]
        [DataMember(Name = DeleteSpecificToNodeRequestDataMemberNames.Levels)]
        public int[] Levels{ get; protected set; }
        public DeleteSpecificToNodeRequest(DatabaseIdentifier databaseIdentifier, long id, int[] levels) : 
            base(InterserverMessageTypes.QuadTreeDeleteSpecificToNode)
        {
            DatabaseIdentifier = databaseIdentifier;
            Id = id;
            Levels = levels;
        }
        protected DeleteSpecificToNodeRequest() :
            base(InterserverMessageTypes.QuadTreeDeleteSpecificToNode)
        { }
    }
}
