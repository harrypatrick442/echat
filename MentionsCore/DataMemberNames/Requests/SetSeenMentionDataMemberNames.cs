using MessageTypes.Attributes;

namespace MentionsCore.DataMemberNames.Requests
{
    [MessageType(MessageTypes.MentionsSetSeen)]
    public static class SetSeenMentionDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON:true)]
        public const string
            UserIdBeingMentioned = "a";
        public const string
            MessageId = "b";
    }
}