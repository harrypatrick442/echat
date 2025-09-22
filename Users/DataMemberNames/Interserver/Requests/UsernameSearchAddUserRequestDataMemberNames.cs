using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Users.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.UsernameSearchAddUser)]
    public static class UsernameSearchAddUserRequestDataMemberNames
    {
        public const string UserId = "i",
            Username = "u";

    }
}