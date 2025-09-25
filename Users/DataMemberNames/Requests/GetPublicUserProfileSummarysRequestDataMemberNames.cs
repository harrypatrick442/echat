using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UsersGetPublicUserProfileSummarys)]
    public static class GetPublicUserProfileSummarysRequestDataMemberNames
    {
        public const string UserIds = "u";
    }
}