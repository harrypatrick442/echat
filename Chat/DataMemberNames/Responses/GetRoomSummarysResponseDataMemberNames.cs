using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class GetRoomSummarysResponseDataMemberNames
    {
        public const string
            Successful = "s";
        [DataMemberNamesClass(typeof(RoomSnapshotDataMemberNames), isArray: true)]
        public const string Summarys = "m";
    }
}