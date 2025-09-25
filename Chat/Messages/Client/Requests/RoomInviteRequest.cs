using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using UsersEnums;
using MultimediaServerCore;
using FileInfo = Core.Messages.Messages.FileInfo;
using Core.DTOs;
using Chat.DataMemberNames.Requests;
namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RoomInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(RoomInviteRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = RoomInviteRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(RoomInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RoomInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(RoomInviteRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = RoomInviteRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get;
            protected set;
        }
        public RoomInviteRequest(long conversationId, long otherUserId, long myUserId)
            : base(MessageTypes.ChatRoomInvite)
        {
            ConversationId = conversationId;
            MyUserId = myUserId;
            OtherUserId = otherUserId;
        }
        public RoomInviteRequest()
            : base(MessageTypes.ChatRoomInvite)
        {
        }
    }
}
