using Core.Messages.Messages;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetAssociateResponse : TicketedMessageBase
    {
        private Associate _Associate;
        [JsonPropertyName(GetAssociateResponseDataMemberNames.Associate)]
        [JsonInclude]
        [DataMember(Name = GetAssociateResponseDataMemberNames.Associate)]
        public Associate Associate
        {
            get { return _Associate; }
            protected set { _Associate = value; }
        }
        public GetAssociateResponse(Associate associate, long ticket) : base(TicketedMessageType.Ticketed)
        {
            _Associate = associate;
            _Ticket = ticket;
        }
        protected GetAssociateResponse() : base(TicketedMessageType.Ticketed)
        { }
    }
}
