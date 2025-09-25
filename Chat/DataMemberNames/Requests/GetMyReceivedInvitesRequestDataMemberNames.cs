using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatGetMyReceivedInvites)]
    public static class GetMyReceivedInvitesRequestDataMemberNames
    {
        public const string
            MyUserId = "u";
    }
}