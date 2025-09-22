using Core.LoadBalancing;
using Core.Ticketing;

namespace FileServerBase
{
    public class EChatIndex : WebsocketLoadBalancingIndexDynamicCachedFile, IDynamicCachedFile
    {
        private static string _MapNodeIdToMultimediaServerDomain;
        static EChatIndex() {

            _MapNodeIdToMultimediaServerDomain = "{"+string.Join(',', GlobalConstants.Nodes.ECHAT_MULTIMEDIA_SERVER_NODES
            .Select(n => new Tuple<int, string>(n, GlobalConstants.Nodes.FirstUniqueDomainForNode(n))).Select(t => $"{t.Item1}:'{t.Item2}'"))
            +"}";
        }
        public EChatIndex(
            string filePath, string requestPath, int defaultNodeId)
            :base(LoadFactorType.EChatWebsocketServer, filePath, requestPath, defaultNodeId,
                 new Tuple<string, Func<string>>[] {
                    new Tuple<string, Func<string>>(
                        GlobalConstants.Placeholders.MAP_NODE_ID_TO_MULTIMEDIA_SERVER_DOMAIN_PLACEHOLDER , ()=>_MapNodeIdToMultimediaServerDomain
                    )
                 })
        {}
    }
}