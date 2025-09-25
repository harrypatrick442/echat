using MessageTypes.Internal;

namespace UserIgnore
{
    public static partial class InterserverMessageTypes
    { 
        public const string UserIgnoresGet = "uig";
        public const string UserIgnoresAdd = "uia";
        public const string UserIgnoresRemove = "uir";
        public const string UserIgnoresAddBeingIgnoredBy = "uiabib";
        public const string UserIgnoresRemoveBeingIgnoredBy = "uirbi";
    }
}