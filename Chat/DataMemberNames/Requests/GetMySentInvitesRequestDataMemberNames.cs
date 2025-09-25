using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatGetMySentInvites)]
    public static class GetMySentInvitesRequestDataMemberNames
    {
        public const string
            MyUserId = "u";
    }
}