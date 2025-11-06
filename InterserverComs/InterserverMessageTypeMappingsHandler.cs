using Core.Handlers;
using Initialization.Exceptions;

namespace InterserverComs
{
    public sealed class InterserverMessageTypeMappingsHandler : MessageTypeMappingsHandler<InterserverMessageEventArgs>
    {
        private static InterserverMessageTypeMappingsHandler _Instance;
        public static InterserverMessageTypeMappingsHandler Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(InterserverMessageTypeMappingsHandler));
            _Instance = new InterserverMessageTypeMappingsHandler();
            return _Instance;
        }
        public static InterserverMessageTypeMappingsHandler Instance { get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(InterserverMessageTypeMappingsHandler));
                return _Instance;
            }
        }
    }
}