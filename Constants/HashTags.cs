namespace GlobalConstants
{
    public static class HashTags
    {
        [ExportToJavaScript]
        public const string ALLOWED_CHARACTERS_STRING_LITERAL = "abcdefghijklmnopqrstuvwxyz0123456789_";

        [ExportToJavaScript]
        public const int MAX_LENGTH = 30;
        public const int MAX_N_SEARCH_ENTRIES = 100;
        public static readonly HashSet<char> ALLOWED_CHARACTERS_HASH_SET = ALLOWED_CHARACTERS_STRING_LITERAL.ToHashSet();
        public static readonly char[] Delimiters = System.Text.Encoding.ASCII.GetString(new byte[] { 32, 13, 9, 188, 190 }).ToCharArray();
    }
}