using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Location.DataMemberNames.Interserver.Responses;

namespace Location.Responses
{
    [DataContract]
    public class SetOnIdAssociatedNodeResponse : TicketedMessageBase
    {
        [JsonPropertyName(SetOnIdAssociatedNodeResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = SetOnIdAssociatedNodeResponseDataMemberNames.Successful)]
        public bool Successful{ get; protected set; }
        public SetOnIdAssociatedNodeResponse(bool successful, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Ticket = ticket;
        }
        protected SetOnIdAssociatedNodeResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static SetOnIdAssociatedNodeResponse Success(long ticket)
        {
            return new SetOnIdAssociatedNodeResponse(true, ticket);
        }
        public static SetOnIdAssociatedNodeResponse Failure(long ticket)
        {
            return new SetOnIdAssociatedNodeResponse(false, ticket);
        }
    }
}
