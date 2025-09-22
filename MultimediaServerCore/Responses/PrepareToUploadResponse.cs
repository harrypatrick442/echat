using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Messages;
using MultimediaServerCore.DataMemberNames.Responses;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class PrepareToUploadResponse : TicketedMessageBase
    {
        [JsonPropertyName(PrepareToUploadResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name =PrepareToUploadResponseDataMemberNames.FailedReason)]
        public MultimediaFailedReason? FailedReason { get; protected set; }
        [JsonPropertyName(PrepareToUploadResponseDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name =PrepareToUploadResponseDataMemberNames.MultimediaToken)]
        public string? MultimediaToken{ get; protected set; }
        private PrepareToUploadResponse(string? multimediaToken,
            MultimediaFailedReason? failedReason, long ticket) : base(TicketedMessageType.Ticketed)
        {
            MultimediaToken = multimediaToken;
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected PrepareToUploadResponse() : base(TicketedMessageType.Ticketed) { }
        public static PrepareToUploadResponse Success(string multimediaToken, 
            long ticket)
        {
            return new PrepareToUploadResponse(multimediaToken, null, ticket);
        }
        public static PrepareToUploadResponse Failure(MultimediaFailedReason failedReason, long ticket)
        {
            return new PrepareToUploadResponse(null, failedReason, ticket);
        }
    }
}
