using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UsersRemoveAssociate)]
    public static class RemoveAssociateRequestDataMemberNames
    {
        [DataMemberNamesIgnore]
        public const string MyUserId = "m";
        public const string OtherUserId = "o";
    }
}