using Core.LoadBalancing;

namespace FileServerBase
{
    public class FilesRelayIndex : WebsocketLoadBalancingIndexDynamicCachedFile, IDynamicCachedFile
    {
        public FilesRelayIndex(
            string filePath, string requestPath, int defaultNodeId)
            :base(LoadFactorType.FilesRelayWebsocketServer, filePath, requestPath, defaultNodeId)
        {

        }
    }
}