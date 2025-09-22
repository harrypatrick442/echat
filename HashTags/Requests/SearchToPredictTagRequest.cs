using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Requests;
using HashTags.Enums;

namespace HashTags.Messages
{
    [DataContract]
    public class SearchToPredictTagRequest : TicketedMessageBase
    {
        [JsonPropertyName(SearchToPredictTagRequestDataMemberNames.Str)]
        [JsonInclude]
        [DataMember(Name = SearchToPredictTagRequestDataMemberNames.Str)]
        public string Str { get; protected set; }
        [JsonPropertyName(SearchToPredictTagRequestDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = SearchToPredictTagRequestDataMemberNames.ScopeType)]
        public HashTagScopeTypes? ScopeType{ get; protected set; }
        [JsonPropertyName(SearchToPredictTagRequestDataMemberNames.MaxNEntries)]
        [JsonInclude]
        [DataMember(Name = SearchToPredictTagRequestDataMemberNames.MaxNEntries)]
        public int MaxNEntries { get; protected set; }
        public SearchToPredictTagRequest(string str, HashTagScopeTypes? scopeType, int maxNEntries)
            :base(global::MessageTypes.MessageTypes.SearchToPredictTag)
        {
            Str = str;
            ScopeType = scopeType;
            MaxNEntries = maxNEntries;
        }
        protected SearchToPredictTagRequest()
            : base(global::MessageTypes.MessageTypes.SearchToPredictTag) { }
    }
}
