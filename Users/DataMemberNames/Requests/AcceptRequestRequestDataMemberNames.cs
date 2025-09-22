using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UsersAcceptRequest)]
    public static class AcceptRequestRequestDataMemberNames
    {
        [DataMemberNamesIgnore]
        public const string MyUserId = "m";
        public const string
            RequestUniqueIdentifier = "u",
            OtherUserId = "o",
            LimitTo = "l";

    }
}