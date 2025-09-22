using Core.LoadBalancing;
using FilesRelayCore.TransferServers;
using Core.Exceptions;
using MultimediaServerCore;

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