using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.DTOs;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class SetAdministratorRequest : TicketedMessageBase
    {
        [JsonPropertyName(SetAdministratorRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = SetAdministratorRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(SetAdministratorRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = SetAdministratorRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(SetAdministratorRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = SetAdministratorRequestDataMemberNames.MyUserId)]
        public long MyUserId { get; protected set; }
        [JsonPropertyName(SetAdministratorRequestDataMemberNames.Privilages)]
        [JsonInclude]
        [DataMember(Name = SetAdministratorRequestDataMemberNames.Privilages)]
        public AdministratorPrivilages Privilages { get; protected set; }
        public SetAdministratorRequest(long conversationId, long myUserId, long userId, AdministratorPrivilages privilages)
            : base(global::MessageTypes.MessageTypes.ChatSetAdministrator)
        {
            ConversationId = conversationId;
            UserId = userId;
            MyUserId = myUserId;
            Privilages = privilages;
        }
        protected SetAdministratorRequest()
            : base(global::MessageTypes.MessageTypes.ChatSetAdministrator) { }
    }
}
