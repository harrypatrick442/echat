using Chat.DataMemberNames.Messages;
using Chat.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class Invite
    {
        [JsonPropertyName(InviteDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = InviteDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(InviteDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = InviteDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(InviteDataMemberNames.SentAt)]
        [JsonInclude]
        [DataMember(Name = InviteDataMemberNames.SentAt)]
        public long SentAt { get; protected set; }
        public Invite(long conversationId, long userId, long sentAt) { 
            ConversationId = conversationId;
            UserId = userId;
            SentAt = sentAt;
        }
        protected Invite() { 
            
        }
    }
}
