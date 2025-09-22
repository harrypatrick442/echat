using HashTags.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HashTags.Messages
{
    [DataContract]
    public class TagWithScopeIds
    {
        [JsonPropertyName(TagWithScopeIdsDataMemberNames.Tag)]
        [JsonInclude]
        [DataMember(Name = TagWithScopeIdsDataMemberNames.Tag)]
        public string Tag { get; protected set; }
        [JsonPropertyName(TagWithScopeIdsDataMemberNames.ScopeId)]
        [JsonInclude]
        [DataMember(Name = TagWithScopeIdsDataMemberNames.ScopeId)]
        public long ScopeId { get; protected set; }
        [JsonPropertyName(TagWithScopeIdsDataMemberNames.ScopeId2)]
        [JsonInclude]
        [DataMember(Name = TagWithScopeIdsDataMemberNames.ScopeId2)]
        public long? ScopeId2 { get; protected set; }
        public TagWithScopeIds(string tag, long scopeId, long? scopeId2)
        {
            Tag = tag;
            ScopeId = scopeId;
            ScopeId2 = scopeId2;
        }
        protected TagWithScopeIds() { }
    }
}
