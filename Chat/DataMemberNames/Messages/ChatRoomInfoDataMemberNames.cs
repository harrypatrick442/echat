using Administration.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class ChatRoomInfoDataMemberNames
    {
        public const string Name = "n";
        [DataMemberNamesClass(typeof(AdministratorDataMemberNames), isArray: true)]
        public const string Administrators = "a";
        public const string ConversationId = "c";
        public const string CreatorUserId = "o";
        public const string NUsers = "u";
        public const string HistoryType = "h";
        public const string Tags = "w";
        public const string MainPicture = "p";
        public const string PendingMainPicture = "q";
        public const string Visibility = "v";
        public const string InvitedUsers = "k";
        public const string JoinedUsers = "j";
        public const string BannedUsers = "b";
    }
}