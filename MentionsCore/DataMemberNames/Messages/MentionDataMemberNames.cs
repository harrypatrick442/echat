using MessageTypes.Attributes;

namespace MentionsCore.DataMemberNames.Messages
{
    [MessageType(MessageTypes.MentionsMention)]
    public static class MentionDataMemberNames
    {
        public const string
            UserIdMentioning = "b",
            AtTime = "c",
            ContentSnapshot = "d",
            MessageId = "e",
            ConversationId = "f",
            Seen = "s";
    }
}