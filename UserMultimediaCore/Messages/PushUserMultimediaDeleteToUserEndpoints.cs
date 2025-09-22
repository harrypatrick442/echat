using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Nodes;
using MessageTypes.Internal;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class PushUserMultimediaDeleteToUserEndpoints : TypedMessageBase
    {
        [JsonPropertyName(PushUserMultimediaDeleteToUserEndpointsDataMemberNames.UserIdSessionIdss)]
        [JsonInclude]
        [DataMember(Name = PushUserMultimediaDeleteToUserEndpointsDataMemberNames.UserIdSessionIdss)]
        public UserIdSessionIds[] UserIdSessionIdss { get; protected set; }
        [JsonPropertyName(PushUserMultimediaDeleteToUserEndpointsDataMemberNames.UserMultimediaDeleteJsonString)]
        [JsonInclude]
        [DataMember(Name = PushUserMultimediaDeleteToUserEndpointsDataMemberNames.UserMultimediaDeleteJsonString)]
        public string UserMultimediaMetadataUpdateJsonString { get; protected set; }
        public PushUserMultimediaDeleteToUserEndpoints(
            UserIdSessionIds[] userIdSessionIdss,
            string userMultimediaMetadataUpdateJsonString)
        {
            Type = InterserverMessageTypes.UserMultimediaPushMetadataUpdateToUserEndpoints;
            UserIdSessionIdss = userIdSessionIdss;
            UserMultimediaMetadataUpdateJsonString = userMultimediaMetadataUpdateJsonString;
        }
        protected PushUserMultimediaDeleteToUserEndpoints() { }
    }
}
