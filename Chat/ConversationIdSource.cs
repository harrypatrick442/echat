using Core.Exceptions;
using Core.Ids;
using DependencyManagement;
using Initialization.Exceptions;
namespace Chat
{
    public sealed class ConversationIdSource : NodeAssignedIdSource
    {
        private static ConversationIdSource _Instance;
        public static ConversationIdSource Instance {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(ConversationIdSource));
                return _Instance;
            } 
        }
        public static ConversationIdSource Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(ConversationIdSource));
            _Instance = new ConversationIdSource();
            return _Instance;
        }
        private ConversationIdSource() : base(DependencyManager.GetString(DependencyNames.ConversationIdSourceDirectory), Configurations.IdTypes.CONVERSATION)
        {

        }
    }
}