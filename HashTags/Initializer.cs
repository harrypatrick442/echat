
using HashTags.DAL;

namespace HashTags
{
    public static class Initializer
    {
        public static void Initialize() {
            HashTagNodeShardMappings.Initialize();
            DalHashTags.Initialize();
            HashTagsMesh.Initialize();
        }
    }
}