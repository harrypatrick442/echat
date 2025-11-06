using Core.Exceptions;
using DependencyManagement;
using Chat;
using HashTags.Messages;
using HashTags.Enums;
using Initialization.Exceptions;
namespace Core.DAL
{
    public class DalMessagesComments : DalMessagesSQLiteMultipleConversationsShards
    {

        private static DalMessagesComments _Instance;/*
        public static DalMessagesComments Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMessagesComments));
                return _Instance;
            }
        }*/
        protected override HashTagScopeTypes _HashTagScopeType => HashTagScopeTypes.Comment;

        public static DalMessagesComments Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMessagesComments));
            _Instance = new DalMessagesComments();
            return _Instance;
        }
        protected DalMessagesComments() :base(DependencyManager.GetString(DependencyNames.MessagesCommentsDatabaseDirectory), 
            ChatConstants.SHARD_SIZE_COMMENTS)
        { 
        
        }
    }
}