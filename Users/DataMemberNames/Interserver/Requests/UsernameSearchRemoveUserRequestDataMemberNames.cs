using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Users.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.UsernameSearchRemoveUser)]
    public static class UsernameSearchRemoveUserRequestDataMemberNames
    {
        public const string UserId = "i";

    }
}