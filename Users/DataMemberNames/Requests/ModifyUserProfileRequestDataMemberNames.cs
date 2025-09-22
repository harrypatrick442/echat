using MessageTypes.Attributes;
using Users.DataMemberNames.Messages;

namespace Users.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UsersModifyUserProfile)]
    public class ModifyUserProfileRequestDataMemberNames
    {
        public const string MyUserId = "u";
        [DataMemberNamesClass(typeof(UserProfileDataMemberNames))]
        public const string UserProfileChanges = "c";
    }
}