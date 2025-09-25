using Core.Messages.Messages;
using MentionsCore.DataMemberNames.Requests;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MentionsCore.Requests
{
    [DataContract]
    public class GetMentionsRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetMentionsRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = GetMentionsRequestDataMemberNames.UserId)]
        public long UserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetMentionsRequestDataMemberNames.IdToExclusive)]
        [JsonInclude]
        [DataMember(Name = GetMentionsRequestDataMemberNames.IdToExclusive)]
        public long? IdToExclusive
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetMentionsRequestDataMemberNames.IdFromInclusive)]
        [JsonInclude]
        [DataMember(Name = GetMentionsRequestDataMemberNames.IdFromInclusive)]
        public long? IdFromInclusive
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetMentionsRequestDataMemberNames.NEntries)]
        [JsonInclude]
        [DataMember(Name = GetMentionsRequestDataMemberNames.NEntries)]
        public int NEntries
        {
            get;
            protected set;
        }
        public GetMentionsRequest(long userId, int nEntries, long? idToExclusive, long? idFromInclusive)
            :base(MessageTypes.MentionsGet)
        {
            UserId = userId;
            NEntries = nEntries;
            IdToExclusive = idToExclusive;
            IdFromInclusive = idFromInclusive;
        }
        protected GetMentionsRequest()
            : base(MessageTypes.MentionsGet) { }
    }
}
