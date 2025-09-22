using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Location.DataMemberNames.Interserver.Responses;

namespace Location.Responses
{
    [DataContract]
    public class DeleteOnIdAssociatedNodeResponse : TicketedMessageBase
    {
        [JsonPropertyName(DeleteOnIdAssociatedNodeResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = DeleteOnIdAssociatedNodeResponseDataMemberNames.Successful)]
        public bool Successful{ get; protected set; }
        public DeleteOnIdAssociatedNodeResponse(bool successful, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Ticket = ticket;
        }
        protected DeleteOnIdAssociatedNodeResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static DeleteOnIdAssociatedNodeResponse Success(long ticket)
        {
            return new DeleteOnIdAssociatedNodeResponse(true, ticket);
        }
        public static DeleteOnIdAssociatedNodeResponse Failure(long ticket)
        {
            return new DeleteOnIdAssociatedNodeResponse(false, ticket);
        }
    }
}
