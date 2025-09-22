
using MessageTypes.Attributes;
using MultimediaCore.DataMemberNames.Messages;
namespace UserMultimediaCore.DataMemberNames.Messages
{
    public class UserMultimediaMetadataUpdateDataMemberNames
    {
        public const string UserId = "u";
        public const string MultimediaType = "t";
        [DataMemberNamesClass(typeof(UserMultimediaItemDataMemberNames), isArray:false)]
        public const string
            UserMultimediaItem = "m";
    }
}