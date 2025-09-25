using MessageTypes.Attributes;
using Core.DataMemberNames;

namespace Logging_ClientFriendly.DataMemberNames.Messages
{
    [MessageType(MessageTypes.LogsLogSession)]
    public class LoggedSessionDataMemberNames
    {
        public const string
            Id = "i",
            AtClientUTC = "ac",
            AtServerUTC = "as",
            Stack = "s",
            StackHash = "sh",
            Message = "m",
            MessageHash = "mh",
            Url = "u",
            Project = "pr",
            Platform = "p",
            Browser = "b",
            NodeId = "n";
    }
}