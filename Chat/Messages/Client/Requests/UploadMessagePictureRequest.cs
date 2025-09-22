using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
using UsersEnums;
using Core.DataMemberNames;
using Chat.DataMemberNames.Requests;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class UploadMessagePictureRequest : TicketedMessageBase
    {
        [JsonPropertyName(UploadMessagePictureRequestDataMemberNames.FileInfo)]
        [JsonInclude]
        [DataMember(Name = UploadMessagePictureRequestDataMemberNames.FileInfo)]
        public FileInfo FileInfo { get; protected set; }
        [JsonPropertyName(UploadMessagePictureRequestDataMemberNames.XRating)]
        [JsonInclude]
        [DataMember(Name = UploadMessagePictureRequestDataMemberNames.XRating)]
        public XRating XRating { get; protected set; }
        [JsonPropertyName(UploadMessagePictureRequestDataMemberNames.VisibleTo)]
        [JsonInclude]
        [DataMember(Name = UploadMessagePictureRequestDataMemberNames.VisibleTo)]
        public VisibleTo VisibleTo { get; protected set; }
        [JsonPropertyName(UploadMessagePictureRequestDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = UploadMessagePictureRequestDataMemberNames.Description)]
        public string Description { get; protected set; }
        public UploadMessagePictureRequest() : base(TicketedMessageType.Ticketed) {

        }
    }
}
