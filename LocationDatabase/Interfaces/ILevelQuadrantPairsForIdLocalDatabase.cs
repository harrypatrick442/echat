using LocationCore;
using System;

namespace Location.Interfaces
{
    public interface ILevelQuadrantPairsForIdLocalDatabase
    {
        void LockOnIdForWrite(long id, Action callback);
        public LevelQuadrantPairsForId Get(long id);
        public void Set(long id, LevelQuadrantPairsForId levelQuadrantPairsForId);
    }
}
