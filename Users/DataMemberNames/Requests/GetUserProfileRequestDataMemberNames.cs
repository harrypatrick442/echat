using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UsersGetUserProfile)]
    public class GetUserProfileRequestDataMemberNames
    {
        public const string ProfileUserId = "p";
    }
}