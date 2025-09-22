using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Timing;
using MultimediaCore.DataMemberNames.Messages;
using MultimediaServerCore.Enums;
using UsersEnums;
namespace MultimediaCore
{
    [DataContract]
    public class UserMultimediaItem
    {
        [JsonPropertyName(UserMultimediaItemDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaItemDataMemberNames.MultimediaToken)]
        public string MultimediaToken { get; protected set; }
        [JsonPropertyName(UserMultimediaItemDataMemberNames.UploadedAt)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaItemDataMemberNames.UploadedAt)]
        public long UploadedAt { get; protected set; }
        [JsonPropertyName(UserMultimediaItemDataMemberNames.XRating)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaItemDataMemberNames.XRating, EmitDefaultValue = false)]
        public XRating? XRating { get; protected set; }
        [JsonPropertyName(UserMultimediaItemDataMemberNames.SetAsMain)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaItemDataMemberNames.SetAsMain, EmitDefaultValue = false)]
        public long? SetAsMain { get; set; }
        [JsonPropertyName(UserMultimediaItemDataMemberNames.VisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaItemDataMemberNames.VisibleTo)]
        public VisibleTo VisibleTo { get; set; }
        [JsonPropertyName(UserMultimediaItemDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaItemDataMemberNames.Description, EmitDefaultValue = false)]
        public string Description { get; set; }
        [JsonPropertyName(UserMultimediaItemDataMemberNames.Status)]
        [JsonInclude]
        [DataMember(Name = UserMultimediaItemDataMemberNames.Status, EmitDefaultValue = false)]
        public MultimediaItemStatus Status { get; set; }
        public UserMultimediaItem(string multimediaToken, long uploadedAt,
            XRating xRating, VisibleTo visibleTo, string description, MultimediaItemStatus status,
            bool setAsMain)
        {
            MultimediaToken = multimediaToken;
            UploadedAt = uploadedAt;
            XRating = xRating;
            VisibleTo = visibleTo;
            Description = description;
            Status = status;
            SetAsMain = setAsMain?uploadedAt:null;
        }
        protected UserMultimediaItem() { }
        public bool IsChildFriendly {
            get {
                return (XRating != null)&&(((XRating)XRating) <= UsersEnums.XRating.ChildFriendly);
            }
        }
        public static UserMultimediaItem NewPending(string multimediaToken,
            XRating xRating, VisibleTo visibleTo, string descriptionRaw, bool setAsMain)
        {
            long uploadedAt = TimeHelper.MillisecondsNow;
            return new UserMultimediaItem(
                    multimediaToken!, uploadedAt, xRating, visibleTo,
                    UserMultimediaConstrainer.Description(descriptionRaw),
                    MultimediaItemStatus.PendingUpload, setAsMain);
        }
    }
}
