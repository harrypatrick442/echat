using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Enums;
using MessageTypes.Internal;
using Location.DataMemberNames.Interserver.Requests;

namespace Location.Requests
{
    [DataContract]
    public class DeleteOnIdAssociatedNodeRequest : TicketedMessageBase
    {
        [JsonPropertyName(DeleteOnIdAssociatedNodeRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = DeleteOnIdAssociatedNodeRequestDataMemberNames.DatabaseIdentifier)]
        public DatabaseIdentifier DatabaseIdentifier { get; protected set; }
        [JsonPropertyName(DeleteOnIdAssociatedNodeRequestDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = DeleteOnIdAssociatedNodeRequestDataMemberNames.Id)]
        public long Id { get; protected set; }
        public DeleteOnIdAssociatedNodeRequest(DatabaseIdentifier databaseIdentifier, long id) : 
            base(InterserverMessageTypes.QuadTreeDeleteOnIdAssociatedNode)
        {
            DatabaseIdentifier = databaseIdentifier;
            Id = id;
        }
        protected DeleteOnIdAssociatedNodeRequest() :
            base(InterserverMessageTypes.QuadTreeDeleteOnIdAssociatedNode)
        { }
    }
}
