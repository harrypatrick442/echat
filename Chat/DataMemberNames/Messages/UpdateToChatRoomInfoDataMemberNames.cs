using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatUpdateRoomInfo)]
    public static class UpdateToChatRoomInfoDataMemberNames
    {
        [DataMemberNamesClass(typeof(ChatRoomInfoDataMemberNames))]
        public const string Info = "i";
    }
}