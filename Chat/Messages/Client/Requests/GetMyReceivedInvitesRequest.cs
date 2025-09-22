using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetMyReceivedInvitesRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetMyReceivedInvitesRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetMyReceivedInvitesRequestDataMemberNames.MyUserId)]
        public long MyUserId { get; protected set; }
        public GetMyReceivedInvitesRequest(long myUserId)
            : base(global::MessageTypes.MessageTypes.ChatGetMyReceivedInvites)
        {
            MyUserId = myUserId;
        }
        protected GetMyReceivedInvitesRequest()
            : base(global::MessageTypes.MessageTypes.ChatGetMyReceivedInvites) { }
    }
}
