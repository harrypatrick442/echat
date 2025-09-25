using Chat.DataMemberNames.Requests;
using Chat.Messages.Client.Messages;
using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetConversationSnapshotsRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(GetConversationSnapshotsRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        [JsonPropertyName(GetConversationSnapshotsRequestDataMemberNames.IdFromInclusive)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsRequestDataMemberNames.IdFromInclusive)]
        public long? IdFromInclusive
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetConversationSnapshotsRequestDataMemberNames.IdToExclusive)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsRequestDataMemberNames.IdToExclusive)]
        public long? IdToExclusive
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetConversationSnapshotsRequestDataMemberNames.NEntries)]
        [JsonInclude]
        [DataMember(Name = GetConversationSnapshotsRequestDataMemberNames.NEntries)]
        public int? NEntries
        {
            get;
            protected set;
        }
        public GetConversationSnapshotsRequest(
            long myUserId, long? idFromInclusive,
            long? idToExclusive, int? nEntries) : base(MessageTypes.ChatGetConversationSnapshots)
        {
            _MyUserId = myUserId;
            IdFromInclusive = idFromInclusive;
            IdToExclusive = idToExclusive;
            NEntries = nEntries;
        }
        protected GetConversationSnapshotsRequest() : base(MessageTypes.ChatGetConversationSnapshots) { }
    }
}
