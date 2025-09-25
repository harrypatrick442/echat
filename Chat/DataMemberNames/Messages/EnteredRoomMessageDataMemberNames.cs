using Chat.DataMemberNames.Responses;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatEnteredRoom)]
    public static class EnteredRoomMessageDataMemberNames
    {
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames), isArray: true)]
        public const string Messages = LoadMessagesHistoryResponseDataMemberNames.Entries;
        [DataMemberNamesClass(typeof(MessageReactionDataMemberNames), isArray: true)]
        public const string Reactions = LoadMessagesHistoryResponseDataMemberNames.Reactions;
        [DataMemberNamesClass(typeof(MessageUserMultimediaItemDataMemberNames), isArray: true)]
        public const string UserMultimediaItems = LoadMessagesHistoryResponseDataMemberNames.UserMultimediaItems;
        public const string OnlineUserIds = "u";
        public const string OnlineRecentlyFirstBufferUserIds = "v";
        public const string OnlineRecentlySecondBufferBufferUserIds = "x";
        public const string OnlineRecentlyLastBufferSwitch = "y";
        public const string Timestamp = "t";
        public const string Successful = LoadMessagesHistoryResponseDataMemberNames.Successful;
        public const string ConversationId = "c";
        [DataMemberNamesClass(typeof(ChatRoomInfoDataMemberNames), isArray: false)]
        public const string Info = "i";
    }
}