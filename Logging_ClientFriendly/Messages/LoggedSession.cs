using Core.Enums;
using Logging_ClientFriendly.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BrowserEnum = Core.Enums.Browser;

namespace Logging_ClientFriendly.Messages
{
    [DataContract]
    public class LoggedSession
    {
        [JsonPropertyName(LoggedSessionDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.Id)]
        public long Id { get; set; }
        [JsonPropertyName(LoggedSessionDataMemberNames.AtClientUTC)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.AtClientUTC)]
        public long AtClientUTC { get; protected set; }
        [JsonPropertyName(LoggedSessionDataMemberNames.AtServerUTC)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.AtServerUTC)]
        public long AtServerUTC { get; set; }
        [JsonPropertyName(LoggedSessionDataMemberNames.Platform)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.Platform)]
        public Platform? Platform { get; protected set; }
        [JsonPropertyName(LoggedSessionDataMemberNames.Browser)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.Browser)]
        public string Browser { get; protected set; }
        [JsonPropertyName(LoggedSessionDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.NodeId)]
        public long? NodeId { get; protected set; }
        [JsonPropertyName(LoggedSessionDataMemberNames.Url)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.Url)]
        public string Url { get; protected set; }
        [JsonPropertyName(LoggedSessionDataMemberNames.Project)]
        [JsonInclude]
        [DataMember(Name = LoggedSessionDataMemberNames.Project)]
        public Project? Project { get; protected set; }
        public BrowserEnum BrowserEnum
        {
            get
            {
                return BrowserHelper.FromString(Browser);
            }
        }
        public LoggedSession(long atClientUTC, Platform platform, string browser,
            Project project, string url, long? nodeId)
        {
            AtClientUTC = atClientUTC;
            Platform = platform;
            Browser = browser;
            Project = project;
            Url = url;
            NodeId = nodeId;
        }
        public LoggedSession(long id, long atClientUTC, Platform platform, string browser,
            Project project, string url, long? nodeId) : this(atClientUTC, platform, browser, project, url, nodeId)
        {
            Id = id;
        }
        protected LoggedSession() { }
    }
}
