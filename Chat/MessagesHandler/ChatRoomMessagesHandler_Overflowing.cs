using Timer = System.Timers.Timer;
using KeyValuePairDatabases;
using JSON;
using System.Timers;
using KeyValuePairDatabases.Interfaces;
using Chat.Interfaces;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace Chat.MessagesHandler
{
    public class ChatRoomMessagesHandler_Overflowing : ChatRoomMessagesHandlerBase
    {
        private long _ConversationId;
        private static IKeyValuePairOnDiskDatabase<long, ChatRoomMessages> _Database
            = OnDiskDatabaseFactory.Create<long, ChatRoomMessages>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.ChatRoomMessagesDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension = ".json"
                }, new IdentifierLock<long>());
        private Timer _TimerFlushToDatabase;
        public ChatRoomMessagesHandler_Overflowing(long conversationId)
            :base(GlobalConstants.Lengths.MAX_N_MESSAGES_IN_OVERFLOWING_CHAT_ROOM)
        {
            _ConversationId= conversationId;
            ChatRoomMessages chatRoomMessages = _Database.Read(conversationId);
            _LatestCachedMessages.Initialize(chatRoomMessages.Messages, chatRoomMessages.Reactions, chatRoomMessages.UserMultimediaItems);
            _TimerFlushToDatabase = new Timer(GlobalConstants.Intervals.CHAT_ROOM_MESSAGE_HANDLER_FLUSH_TO_DATABASE);
            _TimerFlushToDatabase.AutoReset = true;
            _TimerFlushToDatabase.Enabled = true;
            _TimerFlushToDatabase.Elapsed += FlushToDatabaseIfChanged;
            _TimerFlushToDatabase.Start();
        }
        public override void Add(ClientMessage message, out string jsonString)
        {
            jsonString = Json.Serialize(message);
            _LatestCachedMessages.Add(message, jsonString);
#if DEBUG
            FlushMessagesIfChanged();
#endif
        }
        public override void FlushMessagesIfChanged()
        {
            _LatestCachedMessages.FlushIfChanged();
        }
        private void FlushToDatabaseIfChanged(object sender, ElapsedEventArgs e)
        {
            _LatestCachedMessages.FlushIfChanged();
        }
        protected override void FlushMessages(ClientMessage[] messages, MessageReaction[] reactions, MessageUserMultimediaItem[] userMultimediaItems)
        {
            _Database.Write(_ConversationId, new ChatRoomMessages(_ConversationId, messages, reactions, userMultimediaItems));
        }

        public override void ReactToMessage(ReactToMessage reactToMessage)
        {
            _LatestCachedMessages.ReactToMessage(reactToMessage.MessageReaction);
        }

        public override void UnreactToMessage(UnreactToMessage unreactToMessage)
        {
            _LatestCachedMessages.UnreactToMessage(unreactToMessage.MessageReaction);
        }

        public override long[] DeleteMessages(long messageUserId, long[] messageIds,
            bool canDeleteAnyMessage)
        {
            return _LatestCachedMessages.Delete(messageUserId, messageIds, canDeleteAnyMessage);
        }

        public override bool ModifyMessage(ModifyMessage modifyMessageRequest)
        {
            return _LatestCachedMessages.Modify(modifyMessageRequest.Message);
        }

        public override void LoadMessagesHistory(long? idFromInclusive, long? idToExclusive, 
            int? nEntries, out ClientMessage[] messages, out MessageReaction[] reactions, out MessageUserMultimediaItem[] userMultimediaItems)
        {
            throw new NotImplementedException();
        }
    }
}