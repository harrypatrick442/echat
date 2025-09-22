namespace Chat
{
    public static class ConversationSnapshotStringsHelper
    {
        public static string GetLastMessageSubstring(string text) {
            return text;
            //May wish to reduce the length of it in future. 
            if (text == null) return null;
            if (GlobalConstants.Lengths.LAST_MESSAGE_SUBSTRING_MAX_LENGTH > text.Length)
                return text;
            return text?.Substring(0, GlobalConstants.Lengths.LAST_MESSAGE_SUBSTRING_MAX_LENGTH);
        }
    }
}