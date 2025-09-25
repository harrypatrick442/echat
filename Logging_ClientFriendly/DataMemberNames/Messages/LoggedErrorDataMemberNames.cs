using MessageTypes.Attributes;
using Core.DataMemberNames;

namespace Logging_ClientFriendly.DataMemberNames.Messages
{
    [MessageType(MessageTypes.LogsLogError)]
    public class LoggedErrorDataMemberNames
    {
        public const string
            Id = "i",
            SessionId = "si",
            AtClientUTC = "ac",
            AtServerUTC = "as",
            Stack = "s",
            StackHash = "sh",
            Message = "m",
            MessageHash = "mh",
            Platform = "p",
            Browser = "b",
            NodeId = "n";
    }
}