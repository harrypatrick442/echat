using MessageTypes.Attributes;
namespace UserMultimediaCore.DataMemberNames.Requests
{
    [MessageType(MessageTypes.MultimediaDeleteProfilePicture)]
    public class DeleteUserProfilePictureRequestDataMemberNames
    {
        public const string
            MultimediaToken = "m";
    }
}