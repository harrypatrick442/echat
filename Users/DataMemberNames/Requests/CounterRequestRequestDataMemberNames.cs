using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UsersCounterRequest)]
    public static class CounterRequestRequestDataMemberNames
    {
        public const string
            CounterAssociateType = "a";
        [DataMemberNamesIgnore]
        public const string
            MyUserId = "m";
        public const string
            OtherUserId = "o";

    }
}