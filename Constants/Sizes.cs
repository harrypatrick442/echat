namespace GlobalConstants
{
    public static class Sizes
    {
        public const int MAX_OVERFLOWING_CHAT_MESSAGE_SIZE = 10000,
            USER_CONVERSATION_SNAPSHOTS_OLD_MESSAGES_TO_OVERFLOW_LOWER_BOUND = 30,
            USER_CONVERSATION_SNAPSHOTS_OLD_MESSAGES_TO_OVERFLOW_UPPER_BOUND = 60,
            MAX_N_CONNECTIONS_HASH_TAGS_SHARD = 20;

        [ExportToJavaScript]
        public const int MULTIMEDIA_SERVER_MAXIMUM_FILE_SIZE = 50000000;
        public static long GetBytesMemoryAllowedForNode(int nodeId) {
            return 100000000;
        }
    }
}