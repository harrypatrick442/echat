namespace MultimediaServerCore
{
    public enum MultimediaType
    {
        ProfilePicture=1,
        ConversationPicture=2,
        MessagePicture = 3,
        MessageVideo = 4,
        CustomEmoticon =5
    }
    public static class MultimediaTypeExtensions { 
        public static string GetDirectoryName(this MultimediaType type) {
            return ((int)type).ToString();
        }
    }
}