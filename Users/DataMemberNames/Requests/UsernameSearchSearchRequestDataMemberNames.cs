using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Users.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.UsernameSearchSearch)]
    public static class UsernameSearchSearchRequestDataMemberNames
    {
        public const string Str = "s";
        public const string MaxNEntries = "m";
    }
}