namespace Core.DAL
{
    public static class ChatConstants
    {
        public const int SHARD_SIZE_PUBLIC_CHATROOMS = 100;
        public const int SHARD_SIZE_GROUP_CHATS = 100;
        public const int SHARD_SIZE_PMS = 100;
        public const int SHARD_SIZE_WALLS = 1000;
        public const int SHARD_SIZE_COMMENTS = 1000;
        public const int SHARD_SIZE_CONVERSATION_SNAPSHOTS = 1000;
        public const int CONVERSATION_SNAPSHOTS_N_ENTRIES_CACHE = 1000;
    }
}