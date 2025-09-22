using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using MessageTypes.Internal;
using Core.Messages.Messages;
using Users.DataMemberNames.Messages;
using UsersEnums;

namespace Users.Messages.Client
{
    [DataContract]
    public class AssociateUpdate:TypedMessageBase
    {
        [JsonPropertyName(AssociateUpdateDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name =AssociateUpdateDataMemberNames.OtherUserId)]
        public long OtherUserId { get; protected set; }
        [JsonPropertyName(AssociateUpdateDataMemberNames.ActingUserId)]
        [JsonInclude]
        [DataMember(Name = AssociateUpdateDataMemberNames.ActingUserId)]
        public long ActingUserId { get; protected set; }
        [JsonPropertyName(AssociateUpdateDataMemberNames.Operation)]
        [JsonInclude]
        [DataMember(Name = AssociateUpdateDataMemberNames.Operation)]
        public AssociatesOperation Operation { get; protected set; }
        [JsonPropertyName(AssociateUpdateDataMemberNames.AssociateType)]
        [JsonInclude]
        [DataMember(Name = AssociateUpdateDataMemberNames.AssociateType)]
        public AssociateType AssociateType { get; protected set; }
        [JsonPropertyName(AssociateUpdateDataMemberNames.ActingAssociateRequestUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = AssociateUpdateDataMemberNames.ActingAssociateRequestUserProfileSummary)]
        public AssociateRequestUserProfileSummary? ActingAssociateRequestUserProfileSummary { get; protected set; }
        [JsonPropertyName(AssociateUpdateDataMemberNames.OtherUserAssociateRequestUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = AssociateUpdateDataMemberNames.OtherUserAssociateRequestUserProfileSummary)]
        public AssociateRequestUserProfileSummary? OtherUserAssociateRequestUserProfileSummary { get; protected set; }
        [JsonPropertyName(AssociateUpdateDataMemberNames.ActingUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = AssociateUpdateDataMemberNames.ActingUserProfileSummary)]
        public UserProfileSummary? ActingUserProfileSummary { get; protected set; }
        [JsonPropertyName(AssociateUpdateDataMemberNames.OtherUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = AssociateUpdateDataMemberNames.OtherUserProfileSummary)]
        public UserProfileSummary? OtherUserProfileSummary { get; protected set; }
        public AssociateUpdate(AssociatesOperation operation,
            long actingUserId, long otherUserId) : base()
        {
            Operation = operation;
            ActingUserId = actingUserId;
            OtherUserId = otherUserId;
            Type = InterserverMessageTypes.UsersAssociateUpdate;
        }
        public AssociateUpdate(AssociatesOperation operation,
            long actingUserId, long otherUserId, AssociateType associateType) : base()
        {
            Operation = operation;
            ActingUserId = actingUserId;
            OtherUserId = otherUserId;
            AssociateType = associateType;
            Type = InterserverMessageTypes.UsersAssociateUpdate;
        }
        public AssociateUpdate(AssociatesOperation operation,
            long actingUserId, long otherUserId, AssociateType associateType, 
            AssociateRequestUserProfileSummary actingAssociateRequestUserProfileSummary,
            AssociateRequestUserProfileSummary otherUserAssociateRequestUserProfileSummary) : base()
        {
            Operation = operation;
            ActingUserId = actingUserId;
            OtherUserId = otherUserId;
            AssociateType = associateType;
            ActingAssociateRequestUserProfileSummary = actingAssociateRequestUserProfileSummary;
            OtherUserAssociateRequestUserProfileSummary = otherUserAssociateRequestUserProfileSummary;
            Type = InterserverMessageTypes.UsersAssociateUpdate;
        }
        public AssociateUpdate(AssociatesOperation operation,
            long actingUserId, long otherUserId,
            UserProfileSummary actingUserProfileSummary,
            UserProfileSummary otherUserProfileSummary) : base()
        {
            Operation = operation;
            ActingUserId = actingUserId;
            OtherUserId = otherUserId;
            ActingUserProfileSummary = actingUserProfileSummary;
            OtherUserProfileSummary = otherUserProfileSummary;
            Type = InterserverMessageTypes.UsersAssociateUpdate;
        }
        protected AssociateUpdate() { }

    }
}
