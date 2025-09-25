using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UsersAlterAssociate)]
    public static class AlterAssociationRequestDataMemberNames
    {
        public const string
            AssociateType = "a";
        [DataMemberNamesIgnore]
        public const string
            MyUserId = "m";
        public const string
            OtherUserId = "o";

    }
}