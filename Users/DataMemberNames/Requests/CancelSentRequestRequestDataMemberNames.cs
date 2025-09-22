using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UsersCancelSentRequest)]
    public static class CancelSentRequestRequestDataMemberNames
    {
        [DataMemberNamesIgnore]
        public const string MyUserId = "m";
        public const string OtherUserId = "o";

    }
}