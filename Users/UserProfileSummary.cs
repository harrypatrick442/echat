using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Messages;
using UsersEnums;

namespace Users
{
    [DataContract]
    public class UserProfileSummary
    {
        [JsonPropertyName(UserProfileSummaryDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.Username)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.Username)]
        public string Username { get; set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.FirstName)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.FirstName, EmitDefaultValue = false)]
        public string FirstName { get; set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.MiddleNames)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.MiddleNames, EmitDefaultValue =false)]
        public string[] MiddleNames{ get; set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.Surname)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.Surname, EmitDefaultValue = false)]
        public string Surname { get;set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.AboutYou)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.AboutYou, EmitDefaultValue = false)]
        public string AboutYou { get; set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.AssociateType)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.AssociateType, EmitDefaultValue = false)]
        public AssociateType AssociateType { get; set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.MainPicture)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.MainPicture, EmitDefaultValue = false)]
        public string MainPicture { get; set; }
        [JsonPropertyName(UserProfileSummaryDataMemberNames.MainChildFriendlyPicture)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummaryDataMemberNames.MainChildFriendlyPicture, EmitDefaultValue = false)]
        public string MainChildFriendlyPicture { get; set; }
        public UserProfileSummary(long userId, AssociateType associateType) {
            UserId = userId;
            AssociateType = associateType;
        }
        protected UserProfileSummary() { }
    }
}
