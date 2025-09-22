using MessageTypes.Attributes;
using MessageTypes.Internal;
namespace MultimediaServerCore.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.MultimediaPrepareToUpload)]
    public class PrepareToUploadRequestDataMemberNames
    {
        public const string
            NodeIdRequestingUpload = "n",
            FileInfo = "f",
            MultimediaType = "m",
            ScopeType = "s",
            ScopingId = "u",
            ScopingId2 = "v",
            ScopingId3 = "w";
    }
}