using Core.Exceptions;
using DependencyManagement;
using Chat;
using HashTags.Messages;
using HashTags.Enums;
namespace Core.DAL
{
    public class DalMessagesWalls : DalMessagesSQLiteMultipleConversationsShards
    {
        protected override HashTagScopeTypes _HashTagScopeType => HashTagScopeTypes.WallPost;

        private static DalMessagesWalls _Instance;/*
        public static DalMessagesWalls Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMessagesWalls));
                return _Instance;
            }
        }*/
        public static DalMessagesWalls Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMessagesWalls));
            _Instance = new DalMessagesWalls();
            return _Instance;
        }
        protected DalMessagesWalls() :base(DependencyManager.GetString(DependencyNames.MessagesWallsDatabaseDirectory), ChatConstants.SHARD_SIZE_WALLS)
        { 
        
        }
    }
}