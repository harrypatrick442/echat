using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetMyConversationSnapshotsResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetMyConversationSnapshotsResponseDataMemberNames.ConversationSnapshots)]
        [JsonInclude]
        [DataMember(Name = GetMyConversationSnapshotsResponseDataMemberNames.ConversationSnapshots)]
        public ConversationSnapshot[] ConversationSnapshots
        {
            get;
            protected set;
        }
        public GetMyConversationSnapshotsResponse(
            ConversationSnapshot[] conversationSnapshots, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            ConversationSnapshots = conversationSnapshots;
            _Ticket = ticket;
        }
        protected GetMyConversationSnapshotsResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}
