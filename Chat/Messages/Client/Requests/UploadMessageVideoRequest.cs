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
    public class UploadMessageVideoRequest : TicketedMessageBase
    {
        [JsonPropertyName(UploadMessageVideoRequestDataMemberNames.FileInfo)]
        [JsonInclude]
        [DataMember(Name = UploadMessageVideoRequestDataMemberNames.FileInfo)]
        public FileInfo FileInfo { get; protected set; }
        [JsonPropertyName(UploadMessageVideoRequestDataMemberNames.XRating)]
        [JsonInclude]
        [DataMember(Name = UploadMessageVideoRequestDataMemberNames.XRating)]
        public XRating XRating { get; protected set; }
        [JsonPropertyName(UploadMessageVideoRequestDataMemberNames.VisibleTo)]
        [JsonInclude]
        [DataMember(Name = UploadMessageVideoRequestDataMemberNames.VisibleTo)]
        public VisibleTo VisibleTo { get; protected set; }
        [JsonPropertyName(UploadMessageVideoRequestDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = UploadMessageVideoRequestDataMemberNames.Description)]
        public string Description { get; protected set; }
        public UploadMessageVideoRequest() : base(TicketedMessageType.Ticketed) {
            
        }
    }
}
