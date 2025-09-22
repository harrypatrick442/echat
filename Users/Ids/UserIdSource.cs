using Core.Exceptions;
using Core.Ids;
using DependencyManagement;

namespace Users
{
    public sealed class UserIdSource : NodeAssignedIdSource
    {
        private static UserIdSource _Instance;
        public static UserIdSource Instance { get {
                if (_Instance == null) 
                    throw new NotInitializedException(nameof(UserIdSource));
                return _Instance; } }
        public static UserIdSource Initialize()
        {
            if (_Instance != null) 
                throw new AlreadyInitializedException(nameof(UserIdSource));
            _Instance = new UserIdSource();
            return _Instance;
        }
        private UserIdSource() : base(DependencyManager.GetString(DependencyNames.UserIdSourceDirectory), GlobalConstants.IdTypes.USER)
        {

        }
    }
}