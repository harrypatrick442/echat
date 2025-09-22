using Core.Timing;
using DependencyManagement;
namespace MultimediaServerCore
{
    public static class MultimediaTokenHelper
    {
        private const int MULTIMEDIA_TOKEN_VERSION = 1;
        private static readonly char D = Path.DirectorySeparatorChar;
        public static int GetNodeId(string rawMultimediaToken) {
            string[] splits = rawMultimediaToken.Split('_');
            int version = int.Parse(splits[0]);
            if (version == 1)
            {
                return int.Parse(splits[1]);
            }
            throw new NotImplementedException($"Not implemented for version {version}");
        }
        public static string GetThumbnailFileName(string token, string extension) {
            return token + "_" + extension;
        }
        public static string GetMicroRoundedThumbnailFileName(string token, string extension) {
            return token + "_r" + extension;
        }
        public static string[] GetFilePaths(MultimediaToken multimediaToken)
        {
            DateTime dateTime = TimeHelper.GetDateTimeFromMillisecondsUTC(multimediaToken.MillisecondsUTC);
            string dateTimeDirectoris = $"{dateTime.Year}{D}{dateTime.Month}{D}{dateTime.Day}{D}{dateTime.Hour}{D}{dateTime.Minute}";
            string multimediaDirectory = DependencyManager.GetString(DependencyNames.MultimediaDirectory);
            string defaultPath = $"{multimediaDirectory}{Path.DirectorySeparatorChar}{multimediaToken.MultimediaType}{Path.DirectorySeparatorChar}{dateTimeDirectoris}{Path.DirectorySeparatorChar}{multimediaToken.Token}{multimediaToken.Extension}";
            switch (multimediaToken.MultimediaType)
            {
                case MultimediaType.ProfilePicture:
                case MultimediaType.ConversationPicture:
                    string thumbnailPath = $"{multimediaDirectory}{Path.DirectorySeparatorChar}{multimediaToken.MultimediaType}{Path.DirectorySeparatorChar}{dateTimeDirectoris}{Path.DirectorySeparatorChar}{ GetThumbnailFileName(multimediaToken.Token, multimediaToken.Extension)}";
                    return new string[] { defaultPath, thumbnailPath };
                default:
                    return new string[] { defaultPath };
            }
        }
        public static string GetMultimediaTokenAndFilePath(MultimediaType multimediaType, string extension, string token, out string filePath)
        {
            long millisecondsNow = TimeHelper.MillisecondsNow;
            long secondsUTC = ((long)(millisecondsNow / 1000));
            DateTime now = DateTime.UtcNow;
            string dateTimeDirectoris = $"{now.Year}{D}{now.Month}{D}{now.Day}{D}{now.Hour}{D}{now.Minute}";
            string multimediaDirectory = DependencyManager.GetString(DependencyNames.MultimediaDirectory);
            filePath = $"{multimediaDirectory}{Path.DirectorySeparatorChar}{multimediaType}{Path.DirectorySeparatorChar}{dateTimeDirectoris}{Path.DirectorySeparatorChar}";
            string multimediaToken = $"{MULTIMEDIA_TOKEN_VERSION}_{Nodes.Nodes.Instance.MyId}_{(int)multimediaType}_{secondsUTC}_{token}_{extension}";
            return multimediaToken;
        }
    }
}