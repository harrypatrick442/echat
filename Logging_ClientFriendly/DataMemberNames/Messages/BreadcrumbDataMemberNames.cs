using MessageTypes.Attributes;

namespace Logging_ClientFriendly.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.LogsLogBreadcrumb)]
    public static class BreadcrumbDataMemberNames
    {
        public const string
            Id = "i",
            SessionId = "s",
            AtClientUTC = "ac",
            AtServerUTC = "as",
            TypeId = "t",
            Description = "d",
            Value = "v",
            ValueHash = "vh";
    }
}