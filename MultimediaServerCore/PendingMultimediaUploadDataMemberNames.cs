using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
namespace MultimediaServerCore
{
    public class PendingMultimediaUploadDataMemberNames
    {
        public const string
            NodeIdRequestingUpload="n",
            MultimediaType = "m",
            FileInfo = "i",
            FilePath = "p",
            MultimediaToken = "mt",
            ScopeType="st",
            ScopingId = "s",
            ScopingId2 = "s2",
            ScopingId3 = "s3",
            Extension = "e",
            Token = "t";
    }
}
