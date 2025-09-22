using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class GetAnotherUsersAssociatesUserProfileSummarysRequest : TicketedMessageBase
    {
        private long _OtherUserId;
        [JsonPropertyName(GetAnotherUsersAssociatesUserProfileSummarysRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = GetAnotherUsersAssociatesUserProfileSummarysRequestDataMemberNames.OtherUserId)]
        public long OtherUserId { get { return _OtherUserId; } protected set { _OtherUserId = value; } }
        public GetAnotherUsersAssociatesUserProfileSummarysRequest(long otherUserId) : base(global::MessageTypes.MessageTypes.UsersGetAnotherUsersAssociatesUserProfileSummarys)
        {
            _OtherUserId = otherUserId;
        }
        protected GetAnotherUsersAssociatesUserProfileSummarysRequest() : base(global::MessageTypes.MessageTypes.UsersGetAnotherUsersAssociatesUserProfileSummarys)
        { }

    }
}
