using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UsersGetUserProfile)]
    public class GetUserProfileRequestDataMemberNames
    {
        public const string ProfileUserId = "p";
    }
}