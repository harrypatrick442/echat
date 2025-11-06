using Core.LoadBalancing;
using Core.Ticketing;
using WebAbstract.LoadBalancing;
namespace FileServerBase
{
    public class EChatIndex : WebsocketLoadBalancingIndexDynamicCachedFile, IDynamicCachedFile
    {
        private static string _MapNodeIdToMultimediaServerDomain;
        static EChatIndex() {
            var nodesConfiguration = Configurations.Nodes.Instance;
            _MapNodeIdToMultimediaServerDomain = "{"+string.Join(',', Configurations.Nodes.ECHAT_MULTIMEDIA_SERVER_NODES
            .Select(n => new Tuple<int, string>(n, nodesConfiguration.FirstUniqueDomainForNode(n))).Select(t => $"{t.Item1}:'{t.Item2}'"))
            +"}";
        }
        public EChatIndex(
            string filePath, string requestPath, int defaultNodeId)
            :base(LoadFactorType.EChatWebsocketServer, filePath, requestPath, defaultNodeId,
                 new Tuple<string, Func<string>>[] {
                    new Tuple<string, Func<string>>(
                        Configurations.Placeholders.MAP_NODE_ID_TO_MULTIMEDIA_SERVER_DOMAIN_PLACEHOLDER , ()=>_MapNodeIdToMultimediaServerDomain
                    )
                 })
        {}
    }
}