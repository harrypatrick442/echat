namespace GlobalConstants
{
    public class Lengths
    {
        public const int 
            OVERFLOWING_CHAR_HISTORY_LENGTH = 50,
            DEFAULT_MAX_N_ENTRIES_LOAD = 30,
            N_MESSAGES_LOAD_PM = 20, 
            N_MESSAGES_LOAD_GROUP_CHAT = 20,
            N_MESSAGES_LOAD_PUBLIC_CHATROOM=20,
            USER_CONVERSATION_SNAPSHOT_HISTORY_FILE_N_ENTRIES = 30,
            USER_CONVERSATION_SNAPSHOT_N_HISTORY_FILES = 4,
            LAST_MESSAGE_SUBSTRING_MAX_LENGTH = 100;
        [ExportToJavaScript]
        public const int NICKNAME_MAX_LENGTH = 50;
        public const int MAX_N_MESSAGES_IN_OVERFLOWING_CHAT_ROOM = 50;
        public const int MIN_NUMBER_CONVERSATION_SNAPSHOTS_BEFORE_OVERFLOW = 50;
        public const int MAX_N_MOST_ACTIVE_CHATROOMS = 30;
        [ExportToJavaScript]
        public const int MAX_USER_MULTIMEDIA_DESCRIPTION_LENGTH = 300;
        public const int USERNAME_SEARCH_MAX_N_ENTRIES_RESULT = 20;
        public const int MAX_RECENT_ROOMS = 20;
        [ExportToJavaScript]
        public const int MAX_N_MULTIMEDIA_ITEMS_PER_MESSAGE = 12;
    }
}