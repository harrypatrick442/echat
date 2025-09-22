using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetUserRoomsRequest : TicketedMessageBase
    {
        [JsonIgnore]
        public long MyUserId { get; set; }
        [JsonPropertyName(GetUserRoomsRequestDataMemberNames.Operation)]
        [JsonInclude]
        [DataMember(Name = GetUserRoomsRequestDataMemberNames.Operation)]
        public UserRoomsOperation Operation { get; set; }
        public GetUserRoomsRequest(long myUserId, UserRoomsOperation operation)
            : base(global::MessageTypes.MessageTypes.ChatGetUserRooms)
        {
            MyUserId = myUserId;
            Operation = operation;
        }
        protected GetUserRoomsRequest()
            : base(global::MessageTypes.MessageTypes.ChatGetUserRooms) { }
    }
}
