using Chat;
using Core.Exceptions;
using DependencyManagement;
using HashTags.Enums;
using HashTags.Messages;
using Initialization.Exceptions;
namespace Core.DAL
{
    public class DalMessagesPms : DalMessagesSQLiteMultipleConversationsShards
    {
        protected override HashTagScopeTypes _HashTagScopeType => throw new NotImplementedException("Shouldnt be accessing for pms");

        private static DalMessagesPms _Instance;/*
        public static DalMessagesPms Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMessagesPms));
                return _Instance;
            }
        }*/
        public static DalMessagesPms Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMessagesPms));
            _Instance = new DalMessagesPms();
            return _Instance;
        }
        protected DalMessagesPms() :base(DependencyManager.GetString(DependencyNames.MessagesPmsDatabaseDirectory), ChatConstants.SHARD_SIZE_PMS){ 
            
        }
    }
}