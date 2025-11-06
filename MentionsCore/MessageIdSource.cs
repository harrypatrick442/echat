using Core.Exceptions;
using Core.Ids;
using DependencyManagement;
using Initialization.Exceptions;

namespace MentionsCore
{
    public sealed class MentionsIdSource : NodeAssignedIdSource
    {
        private static MentionsIdSource _Instance;
        public static MentionsIdSource Instance {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(MentionsIdSource));
                return _Instance;
            } 
        }
        public static MentionsIdSource Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MentionsIdSource));
            _Instance = new MentionsIdSource();
            return _Instance;
        }
        private MentionsIdSource() : base(DependencyManager.GetString(DependencyNames.MentionIdSourceDirectory), Configurations.IdTypes.MENTION)
        {

        }
    }
}