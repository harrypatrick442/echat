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
    public class CreateRoomResponse : TicketedMessageBase
    {
        [JsonPropertyName(CreateRoomResponseDataMemberNames.Info)]
        [JsonInclude]
        [DataMember(Name = CreateRoomResponseDataMemberNames.Info)]
        public ChatRoomInfo Info { get; protected set; }
        [JsonPropertyName(CreateRoomResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = CreateRoomResponseDataMemberNames.FailedReason)]
        public ChatFailedReason? FailedReason{ get; protected set; }
        public CreateRoomResponse(ChatFailedReason? failedReason, ChatRoomInfo info,  long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Info = info;
            _Ticket = ticket;
        }
        protected CreateRoomResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}
