using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Location.DataMemberNames.Interserver.Responses;

namespace Location.Responses
{
    [DataContract]
    public class SetSpecificToNodeResponse : TicketedMessageBase
    {
        [JsonPropertyName(SetSpecificToNodeResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = SetSpecificToNodeResponseDataMemberNames.Successful)]
        public bool Successful{ get; protected set; }
        public SetSpecificToNodeResponse(bool successful, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Ticket = ticket;
        }
        protected SetSpecificToNodeResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static SetSpecificToNodeResponse Success(long ticket)
        {
            return new SetSpecificToNodeResponse(true, ticket);
        }
        public static SetSpecificToNodeResponse Failure(long ticket)
        {
            return new SetSpecificToNodeResponse(false, ticket);
        }
    }
}
