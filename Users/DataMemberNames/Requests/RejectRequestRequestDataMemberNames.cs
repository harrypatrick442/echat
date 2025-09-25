using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UsersRejectRequest)]
    public static class RejectRequestRequestDataMemberNames
    {
        [DataMemberNamesIgnore]
        public const string MyUserId = "m";
        public const string OtherUserId = "o";

    }
}