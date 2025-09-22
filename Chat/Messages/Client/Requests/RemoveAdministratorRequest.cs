using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.DTOs;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RemoveAdministratorRequest : TicketedMessageBase
    {
        [JsonPropertyName(RemoveAdministratorRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RemoveAdministratorRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(RemoveAdministratorRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = RemoveAdministratorRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(RemoveAdministratorRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = RemoveAdministratorRequestDataMemberNames.MyUserId)]
        public long MyUserId { get; protected set; }
        [JsonPropertyName(RemoveAdministratorRequestDataMemberNames.AllowRemoveOnlyFullAdmin)]
        [JsonInclude]
        [DataMember(Name = RemoveAdministratorRequestDataMemberNames.AllowRemoveOnlyFullAdmin)]
        public bool AllowRemoveOnlyFullAdmin { get; protected set; }
        public RemoveAdministratorRequest(long conversationId, long myUserId, long userId, bool allowRemoveOnlyFullAdmin)
            : base(global::MessageTypes.MessageTypes.ChatRemoveAdministrator)
        {
            ConversationId = conversationId;
            UserId = userId;
            MyUserId = myUserId;
            AllowRemoveOnlyFullAdmin = allowRemoveOnlyFullAdmin;
        }
        protected RemoveAdministratorRequest()
            : base(global::MessageTypes.MessageTypes.ChatRemoveAdministrator) { }
    }
}
