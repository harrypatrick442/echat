namespace LocationCore
{
    public struct LatLngRange
    {
        public double LatFrom { get; }
        public double LatTo { get; }
        public double LngFrom { get; }
        public double LngTo { get; }
        public LatLngRange(double latFrom, double latTo, double lngFrom, double lngTo) {
            LatFrom = latFrom;
            LatTo = latTo;
            LngFrom = lngFrom;
            LngTo = lngTo;
        }
        public LatLng middlePoint
        {
            get
            {
                return new LatLng((LatFrom + LatTo) / 2, (LngFrom + LngTo) / 2);
            }
        }
    }
}
