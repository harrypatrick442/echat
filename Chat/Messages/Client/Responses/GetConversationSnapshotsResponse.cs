using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.Messages.Client.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetConversationSnapshotsResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetConversationSnapshotsResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }

        [JsonPropertyName(GetConversationSnapshotsResponseDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsResponseDataMemberNames.Entries)]
        public ConversationSnapshot[] Entries
        {
            get;
            protected set;
        }

        [JsonPropertyName(GetConversationSnapshotsResponseDataMemberNames.Reactions)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsResponseDataMemberNames.Reactions, EmitDefaultValue =false)]
        public MessageReaction[] Reactions
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetConversationSnapshotsResponseDataMemberNames.UserMultimediaItems)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsResponseDataMemberNames.UserMultimediaItems, EmitDefaultValue =false)]
        public MessageUserMultimediaItem[] UserMultimediaItems
        {
            get;
            protected set;
        }
        private ChatFailedReason _FailedReason;
        [JsonPropertyName(LoadMessagesHistoryResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryResponseDataMemberNames.FailedReason)]
        public ChatFailedReason FailedReason
        {
            get { return _FailedReason; }
            protected set { _FailedReason = value; }
        }
        protected GetConversationSnapshotsResponse(bool successful,
            ConversationSnapshot[] messages, MessageReaction[] reactions,
            MessageUserMultimediaItem[] userMultimediaItems, ChatFailedReason failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Entries = messages;
            Reactions = reactions;
            UserMultimediaItems = userMultimediaItems;
            _FailedReason = failedReason;
            _Ticket = ticket;
        }
        protected GetConversationSnapshotsResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetConversationSnapshotsResponse Success(
            ConversationSnapshot[] messages, MessageReaction[] reactions,
            MessageUserMultimediaItem[] userMultimediaItems,
            ChatFailedReason failedReason, long ticket)
        {
            return new GetConversationSnapshotsResponse(true, messages, reactions, userMultimediaItems, failedReason, ticket);
        }
        public static GetConversationSnapshotsResponse Failed(ChatFailedReason failedReason, long ticket)
        {
            return new GetConversationSnapshotsResponse(false, null, null, null, failedReason, ticket);
        }
    }
}
