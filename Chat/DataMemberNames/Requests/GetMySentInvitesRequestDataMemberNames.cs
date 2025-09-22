using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatGetMySentInvites)]
    public static class GetMySentInvitesRequestDataMemberNames
    {
        public const string
            MyUserId = "u";
    }
}