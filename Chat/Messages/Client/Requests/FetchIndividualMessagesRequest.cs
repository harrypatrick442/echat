using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Chat.Messages.Client.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class FetchIndividualMessagesRequest : TicketedMessageBase
    {
        [JsonPropertyName(FetchIndividualMessagesRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = FetchIndividualMessagesRequestDataMemberNames.MyUserId)]
        public long MyUserId { get; protected set; }
        [JsonPropertyName(FetchIndividualMessagesRequestDataMemberNames.ConversationAndMessageIds)]
        [JsonInclude]
        [DataMember(Name = FetchIndividualMessagesRequestDataMemberNames.ConversationAndMessageIds)]
        public ConversationAndMessageIds[] ConversationAndMessageIds { get; protected set; }
        public FetchIndividualMessagesRequest(
            long myUserId,
            ConversationAndMessageIds[] conversationAndMessageIds)
            : base(global::MessageTypes.MessageTypes.ChatFetchIndividualMessages)
        {
            MyUserId = myUserId;
            ConversationAndMessageIds = conversationAndMessageIds;
        }
        protected FetchIndividualMessagesRequest()
            : base(global::MessageTypes.MessageTypes.ChatFetchIndividualMessages) { }
    }
}
