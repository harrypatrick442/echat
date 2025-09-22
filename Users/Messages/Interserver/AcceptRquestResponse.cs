using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;
using Core.Messages.Responses;
using Users.DataMemberNames.Responses;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class AcceptRquestResponse : SuccessTicketedResponse
    {
        [JsonPropertyName(AcceptRquestResponseDataMemberNames.MyUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = AcceptRquestResponseDataMemberNames.MyUserProfileSummary)]
        public UserProfileSummary MyUserProfileSummary
        {
            get;
            protected set;
        }
        [JsonPropertyName(AcceptRquestResponseDataMemberNames.OtherUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = AcceptRquestResponseDataMemberNames.OtherUserProfileSummary)]
        public UserProfileSummary OtherUserProfileSummary
        {
            get;
            protected set;
        }
        public AcceptRquestResponse(bool success, UserProfileSummary myUserProfileSummary, 
            UserProfileSummary otherUserProfileSummary, long ticket)
            : base(success, ticket)
        {
            MyUserProfileSummary = myUserProfileSummary;
            OtherUserProfileSummary = otherUserProfileSummary;
        }
        protected AcceptRquestResponse()
        { }
    }
}
