using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Location.DataMemberNames.Interserver.Responses;

namespace Location.Responses
{
    [DataContract]
    public class DeleteSpecificToNodeResponse : TicketedMessageBase
    {
        [JsonPropertyName(DeleteSpecificToNodeResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = DeleteSpecificToNodeResponseDataMemberNames.Successful)]
        public bool Successful{ get; protected set; }
        public DeleteSpecificToNodeResponse(bool successful, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Ticket = ticket;
        }
        protected DeleteSpecificToNodeResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static DeleteSpecificToNodeResponse Success(long ticket)
        {
            return new DeleteSpecificToNodeResponse(true, ticket);
        }
        public static DeleteSpecificToNodeResponse Failure(long ticket)
        {
            return new DeleteSpecificToNodeResponse(false, ticket);
        }
    }
}
