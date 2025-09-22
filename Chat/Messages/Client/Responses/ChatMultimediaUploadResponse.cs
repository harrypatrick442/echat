using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MultimediaCore;
using MultimediaServerCore.Enums;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class ChatMultimediaUploadResponse : TicketedMessageBase
    {
        [JsonPropertyName(ChatMultimediaUploadResponseDataMemberNames.UserMultimediaItem)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadResponseDataMemberNames.UserMultimediaItem)]
        public UserMultimediaItem? UserMultimediaItem{ get; protected set; }
        [JsonPropertyName(ChatMultimediaUploadResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadResponseDataMemberNames.FailedReason)]
        public MultimediaFailedReason? FailedReason
        {
            get;
            protected set;
        }
        public ChatMultimediaUploadResponse(UserMultimediaItem? userMultimediaItem,
            MultimediaFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            UserMultimediaItem = userMultimediaItem;
            FailedReason = failedReason;
            _Ticket = ticket;
        }
        protected ChatMultimediaUploadResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}
