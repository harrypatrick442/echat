using Chat.DataMemberNames.Requests;
using Chat.Messages.Client.Messages;
using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class LoadMessagesHistoryRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(LoadMessagesHistoryRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        [JsonPropertyName(LoadMessagesHistoryRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryRequestDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(LoadMessagesHistoryRequestDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryRequestDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        [JsonPropertyName(LoadMessagesHistoryRequestDataMemberNames.IdFromInclusive)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryRequestDataMemberNames.IdFromInclusive)]
        public long? IdFromInclusive
        {
            get;
            protected set;
        }
        [JsonPropertyName(LoadMessagesHistoryRequestDataMemberNames.IdToExclusive)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryRequestDataMemberNames.IdToExclusive)]
        public long? IdToExclusive
        {
            get;
            protected set;
        }
        [JsonPropertyName(LoadMessagesHistoryRequestDataMemberNames.NEntries)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryRequestDataMemberNames.NEntries)]
        public int? NEntries
        {
            get;
            protected set;
        }
        [JsonPropertyName(LoadMessagesHistoryRequestDataMemberNames.MessageChildConversationOptions)]
        [JsonInclude]
        [DataMember(Name = LoadMessagesHistoryRequestDataMemberNames.MessageChildConversationOptions)]
        public MessageChildConversationOptions? MessageChildConversationOptions
        {
            get;
            protected set;
        }
        public LoadMessagesHistoryRequest(long myUserId, long conversationId, long? idFromInclusive,
            long? idToExclusive, int? nEntries) : base(InterserverMessageTypes.ChatLoadMessagesHistory)
        {
            _MyUserId = myUserId;
            ConversationId = conversationId;
            IdFromInclusive = idFromInclusive;
            IdToExclusive = idToExclusive;
            NEntries = nEntries;
        }
        protected LoadMessagesHistoryRequest() : base(InterserverMessageTypes.ChatLoadMessagesHistory) { }
    }
}
