using Core.Interfaces;

namespace Sessions
{
    public static class Initializer
    {
        public static void Initialize()
        {
            SessionsMesh.Initialize();
            SessionIdSource.Initialize();
        }
    }
}