using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Requests;
using HashTags.Enums;

namespace HashTags.Messages
{
    [DataContract]
    public class AddTagsRequest:TicketedMessageBase
    {
        [JsonPropertyName(AddTagsRequestDataMemberNames.Tags)]
        [JsonInclude]
        [DataMember(Name = AddTagsRequestDataMemberNames.Tags)]
        public string[] Tags { get; protected set; }
        [JsonPropertyName(AddTagsRequestDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = AddTagsRequestDataMemberNames.ScopeType)]
        public HashTagScopeTypes ScopeType { get; protected set; }
        [JsonPropertyName(AddTagsRequestDataMemberNames.ScopeId)]
        [JsonInclude]
        [DataMember(Name = AddTagsRequestDataMemberNames.ScopeId)]
        public long ScopeId { get; protected set; }
        [JsonPropertyName(AddTagsRequestDataMemberNames.ScopeId2)]
        [JsonInclude]
        [DataMember(Name = AddTagsRequestDataMemberNames.ScopeId2)]
        public long? ScopeId2 { get; protected set; }
        public AddTagsRequest(string[] tags, HashTagScopeTypes scopeType, long scopeId, long? scopeId2)
            :base(MessageTypes.AddTags)
        {
            Tags = tags;
            ScopeType = scopeType;
            ScopeId = scopeId;
            ScopeId2 = scopeId2;
        }
        protected AddTagsRequest()
            : base(MessageTypes.AddTags) { }
    }
}
