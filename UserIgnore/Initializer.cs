using Core;
using Core.Exceptions;
using KeyValuePairDatabases;

namespace UserIgnore
{
    public static class Initializer
    {
        public static void Initialize() {
            DalUserIgnoresLocal.Initialize();
            UserIgnoresMesh.Initialize();
        }
    }
}
