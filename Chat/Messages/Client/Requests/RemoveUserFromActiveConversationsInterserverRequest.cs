using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Chat.Messages.Client.Messages;
using Core.DTOs;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RemoveUserFromActiveConversationsInternalRequest : TypedMessageBase
    {
        [JsonPropertyName(RemoveUserFromActiveConversationsInternalRequestDataMemberNames.ConversationTypeWithConversationIds_s)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromActiveConversationsInternalRequestDataMemberNames.ConversationTypeWithConversationIds_s)]
        public ConversationTypeWithConversationIds[] ConversationTypeWithConversationIdss { get; protected set; }
        [JsonPropertyName(RemoveUserFromActiveConversationsInternalRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromActiveConversationsInternalRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public RemoveUserFromActiveConversationsInternalRequest(ConversationTypeWithConversationIds[] conversationTypeWithConversationIdss,
            long userId)
        {
            ConversationTypeWithConversationIdss = conversationTypeWithConversationIdss;
            UserId = userId;
            _Type = InterserverMessageTypes.ChatRemoveUserFromActiveConversations;
        }
    }
}
