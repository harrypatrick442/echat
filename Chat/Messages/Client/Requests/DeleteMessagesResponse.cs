using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class DeleteMessagesResponse : TicketedMessageBase
    {
        [JsonPropertyName(DeleteMessagesResponseDataMemberNames.DeletedIds)]
        [JsonInclude]
        [DataMember(Name = DeleteMessagesResponseDataMemberNames.DeletedIds)]
        public long[] DeletedIds
        {
            get;
            protected set;
        }
        public DeleteMessagesResponse(long[] deletedIds, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Ticket = ticket;
            DeletedIds = deletedIds;
        }
        protected DeleteMessagesResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}
