namespace NodeAssignedIdRanges
{
    public static class Initializer
    {
        public static void Initialize(bool isSource)
        {
            if (isSource)
                SourceIdRangesManager.Initialize();
            IdRangesMesh.Initialize();
            NodesIdRangesManager.Initialize();
        }
    }
}