using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
using UsersEnums;
using UserMultimediaCore.DataMemberNames.Requests;

namespace UserMultimediaCore.Requests
{
    [DataContract]
    public class UploadProfilePictureRequest : TicketedMessageBase
    {
        [JsonPropertyName(UploadProfilePictureRequestDataMemberNames.FileInfo)]
        [JsonInclude]
        [DataMember(Name = UploadProfilePictureRequestDataMemberNames.FileInfo)]
        public FileInfo FileInfo { get; protected set; }
        [JsonPropertyName(UploadProfilePictureRequestDataMemberNames.XRating)]
        [JsonInclude]
        [DataMember(Name = UploadProfilePictureRequestDataMemberNames.XRating)]
        public XRating XRating { get; protected set; }
        [JsonPropertyName(UploadProfilePictureRequestDataMemberNames.VisibleTo)]
        [JsonInclude]
        [DataMember(Name = UploadProfilePictureRequestDataMemberNames.VisibleTo)]
        public VisibleTo VisibleTo { get; protected set; }
        [JsonPropertyName(UploadProfilePictureRequestDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = UploadProfilePictureRequestDataMemberNames.Description)]
        public string Description { get; protected set; }
        [JsonPropertyName(UploadProfilePictureRequestDataMemberNames.SetAsMain)]
        [JsonInclude]
        [DataMember(Name = UploadProfilePictureRequestDataMemberNames.SetAsMain)]
        public bool SetAsMain { get; protected set; }
        public UploadProfilePictureRequest(FileInfo fileInfo, XRating xRating, 
            VisibleTo visibleTo, string description, bool setAsMain) : base(MessageTypes.MultimediaUploadProfilePicture)
        {
            FileInfo = fileInfo;
            XRating = xRating;
            VisibleTo = visibleTo;
            Description = description;
            SetAsMain = setAsMain;
        }
        protected UploadProfilePictureRequest() : base(MessageTypes.MultimediaUploadProfilePicture) { }
    }
}
