using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatUpdateRoomInfo)]
    public static class UpdateToChatRoomInfoDataMemberNames
    {
        [DataMemberNamesClass(typeof(ChatRoomInfoDataMemberNames))]
        public const string Info = "i";
    }
}