using Core.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BrowserEnum = Core.Enums.Browser;
using Core.Strings;
using Logging_ClientFriendly.DataMemberNames.Messages;

namespace Logging_ClientFriendly.Messages
{
    [DataContract]
    public class LoggedError
    {
        [JsonPropertyName(LoggedErrorDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.Id)]
        public long? Id { get; set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.SessionId)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.SessionId)]
        public long? SessionId { get; set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.AtClientUTC)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.AtClientUTC)]
        public long? AtClientUTC { get; protected set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.AtServerUTC)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.AtServerUTC)]
        public long? AtServerUTC { get; set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.Stack)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.Stack)]
        public string Stack { get; protected set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.StackHash)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.StackHash)]
        public long StackHash { get; protected set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.Message)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.Message)]
        public string Message { get; protected set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.MessageHash)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.MessageHash)]
        public long MessageHash { get; protected set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.Platform)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.Platform)]
        public Platform? Platform { get; protected set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.Browser)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.Browser)]
        public string Browser { get; protected set; }
        [JsonPropertyName(LoggedErrorDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = LoggedErrorDataMemberNames.NodeId)]
        public long? NodeId { get; protected set; }
        public BrowserEnum BrowserEnum
        {
            get
            {
                return BrowserHelper.FromString(Browser);
            }
        }
        public void CalculateHashes()
        {
            StackHash = Stack == null ? 0 : Stack.ToNonCryptographicHash();
            MessageHash = Message == null ? 0 : Message.ToNonCryptographicHash();
        }
        public LoggedError(long id, long sessionId, long atClientUTC, string stack, string message,
            Platform platform, string browser, long? nodeId) : this(sessionId, atClientUTC, stack, message,
            platform, browser, nodeId)
        {
            Id = id;
        }
        public LoggedError(long sessionId, long atClientUTC, string stack, string message,
            Platform platform, string browser, long? nodeId)
        {
            AtClientUTC = atClientUTC;
            Stack = stack;
            Message = message;
            Platform = platform;
            Browser = browser;
            NodeId = nodeId;
            SessionId = sessionId;
        }
        protected LoggedError() { }
    }
}
