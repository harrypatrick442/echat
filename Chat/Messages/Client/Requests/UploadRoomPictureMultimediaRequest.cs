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
    public class UploadChatRoomPictureRequest : TicketedMessageBase
    {
        [JsonPropertyName(UploadChatRoomPictureRequestDataMemberNames.FileInfo)]
        [JsonInclude]
        [DataMember(Name = UploadChatRoomPictureRequestDataMemberNames.FileInfo)]
        public FileInfo FileInfo { get; protected set; }
        [JsonPropertyName(UploadChatRoomPictureRequestDataMemberNames.XRating)]
        [JsonInclude]
        [DataMember(Name = UploadChatRoomPictureRequestDataMemberNames.XRating)]
        public XRating XRating { get; protected set; }
        [JsonPropertyName(UploadChatRoomPictureRequestDataMemberNames.VisibleTo)]
        [JsonInclude]
        [DataMember(Name = UploadChatRoomPictureRequestDataMemberNames.VisibleTo)]
        public VisibleTo VisibleTo { get; protected set; }
        [JsonPropertyName(UploadChatRoomPictureRequestDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = UploadChatRoomPictureRequestDataMemberNames.Description)]
        public string Description { get; protected set; }
        public UploadChatRoomPictureRequest() : base(TicketedMessageType.Ticketed) { 
            
        }
    }
}
