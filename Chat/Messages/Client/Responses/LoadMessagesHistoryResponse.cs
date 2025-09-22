using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.Messages.Client.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class LoadMessagesHistoryResponse : TicketedMessageBase
    {
        private bool _Successful;
        [JsonPropertyName(LoadMessagesHistoryResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryResponseDataMemberNames.Successful)]
        public bool Successful { get { return _Successful; } protected set { _Successful = value; } }

        [JsonPropertyName(LoadMessagesHistoryResponseDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryResponseDataMemberNames.Entries)]
        public ClientMessage[] Entries
        {
            get;
            protected set;
        }

        [JsonPropertyName(LoadMessagesHistoryResponseDataMemberNames.Reactions)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryResponseDataMemberNames.Reactions, EmitDefaultValue =false)]
        public MessageReaction[] Reactions
        {
            get;
            protected set;
        }
        [JsonPropertyName(LoadMessagesHistoryResponseDataMemberNames.UserMultimediaItems)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryResponseDataMemberNames.UserMultimediaItems, EmitDefaultValue =false)]
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
        protected LoadMessagesHistoryResponse(bool successful,
            ClientMessage[] messages, MessageReaction[] reactions,
            MessageUserMultimediaItem[] userMultimediaItems, ChatFailedReason failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            _Successful = successful;
            Entries = messages;
            Reactions = reactions;
            UserMultimediaItems = userMultimediaItems;
            _FailedReason = failedReason;
            _Ticket = ticket;
        }
        protected LoadMessagesHistoryResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static LoadMessagesHistoryResponse Success(
            ClientMessage[] messages, MessageReaction[] reactions,
            MessageUserMultimediaItem[] userMultimediaItems,
            ChatFailedReason failedReason, long ticket)
        {
            return new LoadMessagesHistoryResponse(true, messages, reactions, userMultimediaItems, failedReason, ticket);
        }
        public static LoadMessagesHistoryResponse Failed(ChatFailedReason failedReason, long ticket)
        {
            return new LoadMessagesHistoryResponse(false, null, null, null, failedReason, ticket);
        }
    }
}
