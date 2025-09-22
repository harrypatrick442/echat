using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatGetRoomSummarys)]
    public static class GetRoomSummarysRequestDataMemberNames
    {
        public const string
            ConversationIds = "c";
    }
}