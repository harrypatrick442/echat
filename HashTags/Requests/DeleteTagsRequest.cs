using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Requests;
using HashTags.Enums;

namespace HashTags.Messages
{
    [DataContract]
    public class DeleteTagsRequest : TicketedMessageBase
    {
        [JsonPropertyName(DeleteTagsRequestDataMemberNames.Tags)]
        [JsonInclude]
        [DataMember(Name = DeleteTagsRequestDataMemberNames.Tags)]
        public string[] Tags { get; protected set; }
        [JsonPropertyName(DeleteTagsRequestDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = DeleteTagsRequestDataMemberNames.ScopeType)]
        public HashTagScopeTypes ScopeType { get; protected set; }
        [JsonPropertyName(DeleteTagsRequestDataMemberNames.ScopeId)]
        [JsonInclude]
        [DataMember(Name = DeleteTagsRequestDataMemberNames.ScopeId)]
        public long ScopeId { get; protected set; }
        [JsonPropertyName(DeleteTagsRequestDataMemberNames.ScopeId2)]
        [JsonInclude]
        [DataMember(Name = DeleteTagsRequestDataMemberNames.ScopeId2)]
        public long? ScopeId2 { get; protected set; }
        public DeleteTagsRequest(HashTagScopeTypes scopeType, long scopeId, long? scopeId2, string[] tags)
            :base(global::MessageTypes.MessageTypes.DeleteTags)
        {
            ScopeType = scopeType;
            ScopeId = scopeId;
            ScopeId2 = scopeId2;
            Tags = tags;
        }
        protected DeleteTagsRequest()
            : base(global::MessageTypes.MessageTypes.DeleteTags) { }
    }
}
