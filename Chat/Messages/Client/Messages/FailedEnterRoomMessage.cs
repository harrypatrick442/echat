using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class FailedEnterRoomMessage : TicketedMessageBase
    {
        [JsonPropertyName(FailedEnterRoomMessageDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = FailedEnterRoomMessageDataMemberNames.FailedReason)]
        public FailedEnterRoomReason FailedReason { get; }
        [JsonPropertyName(FailedEnterRoomMessageDataMemberNames.JoinFailedReason)]
        [JsonInclude]
        [DataMember(Name = FailedEnterRoomMessageDataMemberNames.JoinFailedReason)]
        public JoinFailedReason? JoinFailedReason { get; }
        [JsonPropertyName(FailedEnterRoomMessageDataMemberNames.Visibility)]
        [JsonInclude]
        [DataMember(Name = FailedEnterRoomMessageDataMemberNames.Visibility, EmitDefaultValue =false)]
        public RoomVisibility? Visibility { get; }
        public FailedEnterRoomMessage(FailedEnterRoomReason failedReason, JoinFailedReason? joinFailedReason, RoomVisibility? visibility)
            : base(MessageTypes.ChatFailedEnterRoom)
        {
            FailedReason = failedReason;
            JoinFailedReason = joinFailedReason;
            Visibility = visibility;
        }
        protected FailedEnterRoomMessage()
            : base(MessageTypes.ChatFailedEnterRoom) { }
    }
}
