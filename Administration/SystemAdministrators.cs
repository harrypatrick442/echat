
namespace Chat
{
    public static class SystemAdministrators
    {
        private static readonly Dictionary<long, Administrator>
            _MapUserIdToAdministrator = new Dictionary<long, Administrator> { 
                
            };
        public static bool GetUser(long userId, out Administrator? administrator) { 
            return _MapUserIdToAdministrator.TryGetValue(userId, out administrator);            
        }
        public static bool HasUser(long userId) {
            return _MapUserIdToAdministrator.ContainsKey(userId);
        }
    }
}