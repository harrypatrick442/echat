using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UsersInviteAssociateByUserId)]
    public static class InviteAssociateRequestDataMemberNames
    {
        public const string Email = "e",
            PhoneNumber = "p",
            AssociateType = "a";

    }
}