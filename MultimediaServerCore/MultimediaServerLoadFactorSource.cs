using WebAbstract.LoadBalancing;

namespace MultimediaServerCore
{
    public sealed class MultimediaServerLoadFactorSource : ILoadFactorSource
    {
        public LoadFactorType LoadFactorType => LoadFactorType.MultimediaServer;

        public double GetLoadFactor()
        {
            return PendingMultimediaUploads.Instance.Count;
        }
    }
}