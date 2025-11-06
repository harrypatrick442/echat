using Core.Exceptions;
using DependencyManagement;
using Chat;
using HashTags.Messages;
using HashTags.Enums;
using Initialization.Exceptions;
namespace Core.DAL
{
    public class DalMessagesPublicChatrooms : DalMessagesSQLiteMultipleConversationsShards
    {

        private static DalMessagesPublicChatrooms _Instance;/*
        public static DalMessagesRooms Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMessagesRooms));
                return _Instance;
            }
        }*/
        protected override HashTagScopeTypes _HashTagScopeType => HashTagScopeTypes.ChatRoomMessage;

        public static DalMessagesPublicChatrooms Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMessagesPublicChatrooms));
            _Instance = new DalMessagesPublicChatrooms();
            return _Instance;
        }
        protected DalMessagesPublicChatrooms() :base(DependencyManager.GetString(DependencyNames.MessagesPublicChatroomsDatabaseDirectory), ChatConstants.SHARD_SIZE_PUBLIC_CHATROOMS)
        { 
        
        }
    }
}