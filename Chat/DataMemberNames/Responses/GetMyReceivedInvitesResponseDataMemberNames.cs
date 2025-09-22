using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class GetMyReceivedInvitesResponseDataMemberNames
    {
        public const string Success = "s";
        [DataMemberNamesClass(typeof(InvitesDataMemberNames))]
        public const string Invites = "i";
    }
}