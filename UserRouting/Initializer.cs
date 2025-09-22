using Core.Ids;
using NodeAssignedIdRanges;
using Core.Exceptions;

namespace UserRouting
{
    public static class Initializer
    {
        public static void Initialize(bool debugLoggingEnabled)
        {
            CoreUserRoutingTable.Initialize(debugLoggingEnabled);
        }
    }
}