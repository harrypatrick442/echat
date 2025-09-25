using MessageTypes.Attributes;
using MultimediaCore.DataMemberNames.Messages;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ClientMessage)]
    public class ClientMessageDataMemberNames
    {
        public const string Id = "i";
        public const string Content = "t";
        public const string SentAt = "s";
        [DataMemberNamesIgnore(toJSON: true)]
        public const string UserId = "u";
        [DataMemberNamesClass(typeof(UserMultimediaItemDataMemberNames), isArray: true)]
        public const string UserMultimediaItems = "w";
        public const string ConversationId = "c";
        public const string ConversationType = "e";
        public const string Version = "v";
        public const string MentionUserIds = "m";
        public const string ReplyTo = "r";
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames))]
        [DataMemberNamesIgnore(toJSON: true)]
        public const string ReplyMessage = "d";
        public const string ChildConversationId = "z";
        public const string NChildMessages = "n";
        public const string Tags = "f";
    }
}