using MessageTypes.Attributes;
using Core.DataMemberNames;

namespace Users.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UsersRequestAssociate)]
    public static class RequestAssociateRequestDataMemberNames
    {
        public const string MyUserId = "m",
            OtherUserId = "o",
            AssociateType = "a";

    }
}