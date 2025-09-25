using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
using UsersEnums;
using UserMultimediaCore.DataMemberNames.Requests;

namespace UserMultimediaCore.Requests
{
    [DataContract]
    public class UpdateProfilePictureMetadataRequest : TicketedMessageBase
    {
        [JsonPropertyName(UpdateProfilePictureMetadataRequestDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = UpdateProfilePictureMetadataRequestDataMemberNames.MultimediaToken)]
        public string MultimediaToken { get; protected set; }
        [JsonPropertyName(UpdateProfilePictureMetadataRequestDataMemberNames.VisibleTo)]
        [JsonInclude]
        [DataMember(Name = UpdateProfilePictureMetadataRequestDataMemberNames.VisibleTo)]
        public VisibleTo VisibleTo { get; protected set; }
        [JsonPropertyName(UpdateProfilePictureMetadataRequestDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = UpdateProfilePictureMetadataRequestDataMemberNames.Description)]
        public string Description { get; protected set; }
        [JsonPropertyName(UpdateProfilePictureMetadataRequestDataMemberNames.SetAsMain)]
        [JsonInclude]
        [DataMember(Name = UpdateProfilePictureMetadataRequestDataMemberNames.SetAsMain)]
        public bool SetAsMain { get; protected set; }
        public UpdateProfilePictureMetadataRequest(string multimediaToken, VisibleTo visibleTo,
            string description, bool setAsMain) : base(MessageTypes.MultimediaUpdateProfilePictureMetadata)
        {
            MultimediaToken = multimediaToken;
            VisibleTo = visibleTo;
            Description = description;
            SetAsMain = setAsMain;
        }
        protected UpdateProfilePictureMetadataRequest() : base(MessageTypes.MultimediaUpdateProfilePictureMetadata)
        { }
    }
}
