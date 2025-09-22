using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.Interfaces;
using UsersEnums;

namespace Users
{
    public static class Initializer
    {
        public static void Initialize()
        {
            RequestAssociateUniqueIdentifierSource.Initialize();
            UserIdSource.Initialize();
            UserIdToNodeId.Initialize();
            UsersMesh.Initialize();
        }
    }
}
