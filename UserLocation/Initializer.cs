using Core.Enums;
using Core.Exceptions;
using DependencyManagement;
using LocationCore;
using LocationDatabase;
using Users;

namespace UserLocation
{
    public static class Initializer
    {
        public static void Initialize()
        {
            UserQuadTree.Initialize();
        }
    }
}