using MentionsCore.DataMemberNames.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace MentionsCore.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.MentionsAddOrUpdate)]
    public static class AddOrUpdateMentionDataMemberNames
    {
        [DataMemberNamesIgnore]
        public const string UserIdsBeingMentioned = "a";
        public const string IsUpdate = "u";
        [DataMemberNamesClass(typeof(MentionDataMemberNames))]
        public const string
            Mention = "b";
    }
}