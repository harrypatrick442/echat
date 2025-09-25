using MessageTypes.Attributes;
namespace UserMultimediaCore.DataMemberNames.Requests
{
    [MessageType(MessageTypes.MultimediaUpdateProfilePictureMetadata)]
    public class UpdateProfilePictureMetadataRequestDataMemberNames
    {
        public const string MultimediaToken = "m";
        public const string VisibleTo = "v";
        public const string Description = "d";
        public const string SetAsMain = "s";
    }
}