using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MultimediaCore;
using UserMultimediaCore.DataMemberNames.Messages;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class UserMultimediaMetadataUpdate : TicketedMessageBase
    {
        [JsonPropertyName(UserMultimediaMetadataUpdateDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaMetadataUpdateDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(UserMultimediaMetadataUpdateDataMemberNames.MultimediaType)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaMetadataUpdateDataMemberNames.MultimediaType)]
        public MultimediaType MultimediaType { get; protected set; }
        [JsonPropertyName(UserMultimediaMetadataUpdateDataMemberNames.UserMultimediaItem)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaMetadataUpdateDataMemberNames.UserMultimediaItem)]
        public UserMultimediaItem UserMultimediaItem { get; protected set; }
        public UserMultimediaMetadataUpdate(long userId, MultimediaType multimediaType,
            UserMultimediaItem userMultimediaItem) : base(
            global::MessageTypes.MessageTypes.MultimediaMetadataUpdate)
        {
            UserId = userId;
            MultimediaType = multimediaType;
            UserMultimediaItem = userMultimediaItem;
        }
        protected UserMultimediaMetadataUpdate() : base(
            global::MessageTypes.MessageTypes.MultimediaMetadataUpdate)
        { }
    }
}
