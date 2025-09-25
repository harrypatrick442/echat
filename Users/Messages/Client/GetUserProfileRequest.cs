using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class GetUserProfileRequest : TicketedMessageBase
    {
        private long _ProfileUserId;
        [JsonPropertyName(GetUserProfileRequestDataMemberNames.ProfileUserId)]
        [JsonInclude]
        [DataMember(Name = GetUserProfileRequestDataMemberNames.ProfileUserId)]
        public long ProfileUserId { get { return _ProfileUserId; } protected set { _ProfileUserId = value; } }
        public GetUserProfileRequest(long ProfileUserId) : base(MessageTypes.UsersGetUserProfile)
        {
            _ProfileUserId = ProfileUserId;
        }
        protected GetUserProfileRequest() : base(MessageTypes.UsersGetUserProfile)
        { }

    }
}
