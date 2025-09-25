using MessageTypes.Attributes;
using Chat.DataMemberNames.Messages;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatLoadMessagesHistory)]
    public static class LoadMessagesHistoryRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string
            MyUserId = "i";
        public const string
            ConversationId = "t",
            ConversationType = "u",
            IdFromInclusive = "m",
            IdToExclusive = "n",
            NEntries = "o";
        [DataMemberNamesClass(typeof(MessageChildConversationOptionsDataMemberNames))]
        public const string
            MessageChildConversationOptions = "z";
    }
}