using Core.Messages.Messages;
using MessageTypes.Internal;
using MultimediaServerCore.DataMemberNames.Messages;
using Nodes;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserMultimediaCore.DataMemberNames.Messages;
using FileInfo = Core.Messages.Messages.FileInfo;
namespace UserMultimediaCore.Messages
{
    [DataContract]
    public class PushMultimediaUploadToUserEndpoints : TypedMessageBase
    {
        [JsonPropertyName(PushMultimediaUploadToUserEndpointsDataMemberNames.UserIdSessionIdss)]
        [JsonInclude]
        [DataMember(Name= PushMultimediaUploadToUserEndpointsDataMemberNames.UserIdSessionIdss)]
        public UserIdSessionIds[] UserIdSessionIdss { get; protected set; }
        [JsonPropertyName(PushMultimediaUploadToUserEndpointsDataMemberNames.SuccessfulMultimediaUploadJsonString)]
        [JsonInclude]
        [DataMember(Name = PushMultimediaUploadToUserEndpointsDataMemberNames.SuccessfulMultimediaUploadJsonString)]
        public string SuccessfulMultimediaUploadJsonString { get; protected set; }

        public PushMultimediaUploadToUserEndpoints(
            UserIdSessionIds[] userIdSessionIdss, string successfulMultimediaUploadJsonString):base()
        {
            UserIdSessionIdss = userIdSessionIdss;
            SuccessfulMultimediaUploadJsonString = successfulMultimediaUploadJsonString;
            Type = InterserverMessageTypes.UserMultimediaPushToUserEndpoints;
        }
        protected PushMultimediaUploadToUserEndpoints() { }
    }
}
