using Core.Enums;

namespace Location.Interfaces
{
    public interface IQuadTreeDatabase
    {
        DatabaseIdentifier Identifier { get; }
        int NLevels { get; }
        int GetNodeIdForId(long id);
        int GetNodeIdForLevelAndQuadrant(int level, long quadrant);
        IQuadrantsLocalDatabase QuadrantsLocalDatabase { get; }
        ILevelQuadrantPairsForIdLocalDatabase LevelQuadrantPairsForIdLocalDatabase { get; }
    }
}
