using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class MessageReactionDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string UserId = "u";
        public const string Code = "c";
        public const string MessageId = "i";
        public const string MultimediaToken = "m";
    }
}