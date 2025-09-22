using MessageTypes.Attributes;
using MultimediaCore.DataMemberNames.Messages;
namespace MultimediaServerCore.DataMemberNames.Responses
{
    public class UploadMultimediaResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(UserMultimediaItemDataMemberNames), isArray: false)]
        public const string
            UserMultimediaItem = "m";
        public const string
            FailedReason = "f";
    }
}