using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class InvitesDataMemberNames
    {
        [DataMemberNamesClass(typeof(InviteDataMemberNames), isArray: true)]
        public const string Entries = "e";
    }
}