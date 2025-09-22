using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using MultimediaServerCore.Enums;
using MultimediaServerCore.DataMemberNames.Responses;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class GenericMultimediaResponse : TicketedMessageBase
    {
        [JsonPropertyName(GenericMultimediaResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = GenericMultimediaResponseDataMemberNames.FailedReason)]
        public MultimediaFailedReason? FailedReason{ get; protected set; }
        public GenericMultimediaResponse(MultimediaFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected GenericMultimediaResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}
