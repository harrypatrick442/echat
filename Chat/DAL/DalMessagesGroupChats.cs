using Core.Exceptions;
using DependencyManagement;
using Chat;
using HashTags.Messages;
using HashTags.Enums;
namespace Core.DAL
{
    public class DalMessagesGroupChats : DalMessagesSQLiteMultipleConversationsShards
    {

        protected override HashTagScopeTypes _HashTagScopeType => throw new NotImplementedException();
        private static DalMessagesGroupChats _Instance;/*
        public static DalMessagesRooms Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMessagesRooms));
                return _Instance;
            }
        }*/
        public static DalMessagesGroupChats Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMessagesGroupChats));
            _Instance = new DalMessagesGroupChats();
            return _Instance;
        }
        protected DalMessagesGroupChats() :base(DependencyManager.GetString(DependencyNames.MessagesGroupChatsDatabaseDirectory), ChatConstants.SHARD_SIZE_GROUP_CHATS)
        { 
        
        }
    }
}