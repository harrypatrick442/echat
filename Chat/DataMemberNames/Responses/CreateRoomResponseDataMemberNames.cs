using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat.DataMemberNames.Responses
{
    public static class CreateRoomResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(ChatRoomInfoDataMemberNames))]
        public const string Info = "i";
        public const string FailedReason = "f";
    }
}
