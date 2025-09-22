namespace LocationCore
{
    public struct IdAndLevels
    {
        public long Id { get; }
        public int[] Levels { get; }
        public IdAndLevels(long id, int[] levels) {
            Id = id;
            Levels = levels;
        }
    }
}
