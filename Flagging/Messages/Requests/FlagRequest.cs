using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Flagging.DataMemberNames.Requests;
using Flagging.Enums;

namespace Flagging.Messages.Requests
{
    [DataContract]
    public class FlagRequest : TicketedMessageBase
    {
        [JsonPropertyName(FlagRequestDataMemberNames.UserIdFlagging)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.UserIdFlagging)]
        public long UserIdFlagging { get; set; }
        [JsonPropertyName(FlagRequestDataMemberNames.UserIdBeingFlagged)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.UserIdBeingFlagged)]
        public long UserIdBeingFlagged { get; protected set; }
        [JsonPropertyName(FlagRequestDataMemberNames.FlagType)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.FlagType)]
        public FlagType FlagType { get; protected set; }
        [JsonPropertyName(FlagRequestDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.ScopeType)]
        public long ScopeType { get; protected set; }
        [JsonPropertyName(FlagRequestDataMemberNames.ScopeId)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.ScopeId)]
        public long ScopeId { get; protected set; }
        [JsonPropertyName(FlagRequestDataMemberNames.ScopeId2)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.ScopeId2)]
        public long? ScopeId2 { get; protected set; }
        [JsonPropertyName(FlagRequestDataMemberNames.FlaggedAt)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.FlaggedAt)]
        public long FlaggedAt { get; set; }
        [JsonPropertyName(FlagRequestDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = FlagRequestDataMemberNames.Description)]
        public string Description { get; protected set; }
        public FlagRequest(long myUserId, long userIdBeingFlagged, 
            FlagType flagType, long scopeType, long scopeId, long? scopeId2, 
            long flaggedAt, string description)
            : base(MessageTypes.FlaggingFlag)
        {
            UserIdFlagging = myUserId;
            UserIdBeingFlagged = userIdBeingFlagged;
            FlagType = flagType;
            ScopeType = scopeType;
            ScopeId = scopeId;
            ScopeId2 = scopeId2;
            FlaggedAt = flaggedAt;
            Description = description;
        }
        protected FlagRequest()
            : base(MessageTypes.FlaggingFlag) { 
            
        }
    }
}
