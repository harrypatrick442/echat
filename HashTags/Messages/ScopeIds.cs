using HashTags.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HashTags.Messages
{
    [DataContract]
    public class ScopeIds
    {
        [JsonPropertyName(ScopeIdsDataMemberNames.ScopeId)]
        [JsonInclude]
        [DataMember(Name = ScopeIdsDataMemberNames.ScopeId)]
        public long ScopeId { get; protected set; }
        [JsonPropertyName(ScopeIdsDataMemberNames.ScopeId2)]
        [JsonInclude]
        [DataMember(Name = ScopeIdsDataMemberNames.ScopeId2)]
        public long? ScopeId2 { get; protected set; }
        public ScopeIds(long scopeId, long? scopeId2)
        {
            ScopeId = scopeId;
            ScopeId2 = scopeId2;
        }
        protected ScopeIds() { }
    }
}
