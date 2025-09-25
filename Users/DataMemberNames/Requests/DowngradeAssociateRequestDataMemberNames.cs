using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UsersDowngradeAssociation)]
    public static class DowngradeAssociateRequestDataMemberNames
    {
        public const string AssociationTypesToKeep = "a";
        [DataMemberNamesIgnore]
        public const string MyUserId = "m";
        public const string OtherUserId = "o";

    }
}