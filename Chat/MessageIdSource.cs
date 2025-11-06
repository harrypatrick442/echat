using Core.Exceptions;
using Core.Ids;
using DependencyManagement;
using Initialization.Exceptions;

namespace Chat
{
    public sealed class MessageIdSource : NodeAssignedIdSource
    {
        private static MessageIdSource _Instance;
        public static MessageIdSource Instance {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(MessageIdSource));
                return _Instance;
            } 
        }
        public static MessageIdSource Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MessageIdSource));
            _Instance = new MessageIdSource();
            return _Instance;
        }
        private MessageIdSource() : base(DependencyManager.GetString(DependencyNames.MessageIdSourceDirectory), Configurations.IdTypes.MESSAGE)
        {

        }
    }
}