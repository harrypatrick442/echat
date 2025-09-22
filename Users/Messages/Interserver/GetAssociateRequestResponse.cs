using Core.Messages.Messages;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetAssociateRequestResponse : TicketedMessageBase
    {
        private AssociateRequest _AssociateRequest;
        [JsonPropertyName(GetAssociateRequestResponseDataMemberNames.AssociateRequest)]
        [JsonInclude]
        [DataMember(Name = GetAssociateRequestResponseDataMemberNames.AssociateRequest)]
        public AssociateRequest AssociateRequest
        {
            get { return _AssociateRequest; }
            protected set { _AssociateRequest = value; }
        }
        public GetAssociateRequestResponse(AssociateRequest associateRequest, long ticket) : base(TicketedMessageType.Ticketed)
        {
            _AssociateRequest = associateRequest;
            _Ticket = ticket;
        }
        protected GetAssociateRequestResponse() : base(TicketedMessageType.Ticketed)
        { }
    }
}
