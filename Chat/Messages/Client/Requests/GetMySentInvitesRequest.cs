using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetMySentInvitesRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetMySentInvitesRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetMySentInvitesRequestDataMemberNames.MyUserId)]
        public long MyUserId { get; protected set; }
        public GetMySentInvitesRequest(long myUserId)
            : base(global::MessageTypes.MessageTypes.ChatGetMySentInvites)
        {
            MyUserId = myUserId;
        }
        protected GetMySentInvitesRequest()
            : base(global::MessageTypes.MessageTypes.ChatGetMySentInvites) { }
    }
}
