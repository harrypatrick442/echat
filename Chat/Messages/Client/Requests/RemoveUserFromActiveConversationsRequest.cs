using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RemoveUserFromActiveConversationsRequest : TypedMessageBase
    {
        [JsonPropertyName(RemoveUserFromActiveConversationsRequestDataMemberNames.ConversationIds)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromActiveConversationsRequestDataMemberNames.ConversationIds)]
        public long[] ConversationIds{ get; protected set; }
        [JsonPropertyName(RemoveUserFromActiveConversationsRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromActiveConversationsInternalRequestDataMemberNames.UserId)]
        public long UserId { get; set; }
        public RemoveUserFromActiveConversationsRequest(long[] conversationIds,
            long userId)
        {
            ConversationIds = conversationIds;
            UserId = userId;
            _Type = InterserverMessageTypes.ChatRemoveUserFromActiveConversations;
        }
        private RemoveUserFromActiveConversationsRequest() { 
        
        }
    }
}
