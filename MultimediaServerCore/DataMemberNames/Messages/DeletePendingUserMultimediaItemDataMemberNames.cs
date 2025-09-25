using MessageTypes.Attributes;
namespace MultimediaServerCore.DataMemberNames.Messages
{
    [MessageType(MessageTypes.MultimediaDeletePending)]
    public class DeletePendingUserMultimediaItemDataMemberNames
    {
        public const string
            MultimediaToken = "m";
    }
}