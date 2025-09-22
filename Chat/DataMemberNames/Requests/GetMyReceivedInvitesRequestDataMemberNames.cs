using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatGetMyReceivedInvites)]
    public static class GetMyReceivedInvitesRequestDataMemberNames
    {
        public const string
            MyUserId = "u";
    }
}