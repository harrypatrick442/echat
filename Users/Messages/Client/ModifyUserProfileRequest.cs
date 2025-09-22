using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class ModifyUserProfileRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(ModifyUserProfileRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = ModifyUserProfileRequestDataMemberNames.MyUserId)]
        public long MyUserId { get { return _MyUserId; } protected set { _MyUserId = value; } }
        private UserProfile _UserProfileChanges;
        [JsonPropertyName(ModifyUserProfileRequestDataMemberNames.UserProfileChanges)]
        [JsonInclude]
        [DataMember(Name = ModifyUserProfileRequestDataMemberNames.UserProfileChanges)]
        public UserProfile UserProfileChanges { get { return _UserProfileChanges; } protected set { _UserProfileChanges = value; } }
        public ModifyUserProfileRequest(long myUserId, UserProfile userProfile) : 
            base(global::MessageTypes.MessageTypes.UsersModifyUserProfile)
        {
            _MyUserId = myUserId;
            _UserProfileChanges = userProfile;
        }
        protected ModifyUserProfileRequest() :
            base(global::MessageTypes.MessageTypes.UsersModifyUserProfile)
        { }

    }
}
