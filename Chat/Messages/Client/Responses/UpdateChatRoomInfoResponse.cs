using Core.DataMemberNames;
using System.Runtime.Serialization;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class UpdateChatRoomInfoResponse : TicketedMessageBase
    {
        public UpdateChatRoomInfoResponse(long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            _Ticket = ticket;
        }
        protected UpdateChatRoomInfoResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}
