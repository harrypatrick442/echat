using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Nodes;
using MessageTypes.Internal;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class PushUserMultimediaMetadataUpdateToUserEndpoints:TypedMessageBase
    {
        [JsonPropertyName(PushUserMultimediaMetadataUpdateToUserEndpointsDataMemberNames.UserIdSessionIdss)]
        [JsonInclude]
        [DataMember(Name = PushUserMultimediaMetadataUpdateToUserEndpointsDataMemberNames.UserIdSessionIdss)]
        public UserIdSessionIds[] UserIdSessionIdss { get; protected set; }
        [JsonPropertyName(PushUserMultimediaMetadataUpdateToUserEndpointsDataMemberNames.UserMultimediaMetadataUpdateJsonString)]
        [JsonInclude]
        [DataMember(Name = PushUserMultimediaMetadataUpdateToUserEndpointsDataMemberNames.UserMultimediaMetadataUpdateJsonString)]
        public string UserMultimediaMetadataUpdateJsonString { get; protected set; }
        public PushUserMultimediaMetadataUpdateToUserEndpoints(
            UserIdSessionIds[] userIdSessionIdss, 
            string userMultimediaMetadataUpdateJsonString) 
        {
            Type = InterserverMessageTypes.UserMultimediaPushMetadataUpdateToUserEndpoints;
            UserIdSessionIdss = userIdSessionIdss;
            UserMultimediaMetadataUpdateJsonString = userMultimediaMetadataUpdateJsonString;
        }
        protected PushUserMultimediaMetadataUpdateToUserEndpoints() { }
    }
}
