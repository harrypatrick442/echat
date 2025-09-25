using LocationCore;

namespace Location.Interfaces
{
    public interface IQuadrantsLocalDatabase
    {
        public Quadrant[] Get(LevelQuadrantPair[] levelQuadrantPairs);
        public QuadrantNEntries[] GetNEntries(int level, long[] quadrants, bool withLatLng);
        public void Set(long id, double lat, double lng, LevelQuadrantPair[] levelQuadrantPairs);
        public void Delete(long id, int[] levels);
    }
}
