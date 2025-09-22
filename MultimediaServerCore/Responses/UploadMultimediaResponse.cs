using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using MultimediaServerCore.Enums;
using MultimediaCore;
using MultimediaServerCore.DataMemberNames.Responses;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class UploadMultimediaResponse : TicketedMessageBase
    {
        [JsonPropertyName(UploadMultimediaResponseDataMemberNames.UserMultimediaItem)]
        [JsonInclude]
        [DataMember(Name = UploadMultimediaResponseDataMemberNames.UserMultimediaItem)]
        public UserMultimediaItem? UserMultimediaItem{ get; protected set; }
        [JsonPropertyName(UploadMultimediaResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = UploadMultimediaResponseDataMemberNames.FailedReason)]
        public MultimediaFailedReason? FailedReason{ get; protected set; }
        public UploadMultimediaResponse(MultimediaFailedReason? failedReason,
            UserMultimediaItem? userMultimediaItem, long ticket) : base(TicketedMessageType.Ticketed)
        {
            UserMultimediaItem = userMultimediaItem;
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected UploadMultimediaResponse() : base(TicketedMessageType.Ticketed) { }
        public static UploadMultimediaResponse Successful(UserMultimediaItem userMultimediaItem, long ticket)
        {
            return new UploadMultimediaResponse(null, userMultimediaItem, ticket);
        }
        public static UploadMultimediaResponse Failed(MultimediaFailedReason? failedReason, long ticket)
        {
            return new UploadMultimediaResponse(failedReason, null, ticket);
        }
    }
}
