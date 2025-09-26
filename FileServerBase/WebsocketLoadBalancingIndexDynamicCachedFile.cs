using Core.LoadBalancing;
using WebAbstract.LoadBalancing;
using Timer = System.Timers.Timer;

namespace FileServerBase
{
    public abstract class WebsocketLoadBalancingIndexDynamicCachedFile : ReceivingLoadBalancerBase, IDynamicCachedFile
    {
        public string RequestPath { get; }
        public bool IsIndex { get { return true; } }

        private int _CurrentIndex = 0;
        private string _ContentType;
        private byte[] _DefaultBytes;
        private IndexFileWithInserts _IndexFileWithInsert;
        private Dictionary<int, byte[]> _MapNodeIdToBytes = new Dictionary<int, byte[]>();
        private readonly object _LockObjectSequenceToHandleOutForProportion = new object();
        private byte[][] _SequenceToHandOutForProportion;
        private Timer _TimerScheduledUpdate;
        private Func<int, byte[]> _GetBytes;
        public WebsocketLoadBalancingIndexDynamicCachedFile(LoadFactorType loadFactorType, 
            string filePath, string requestPath, int defaultNodeId, 
            Tuple<string, Func<string>>[] additionalPlaceholderAndGetValue_s = null)
            :base(loadFactorType,
                GlobalConstants.Intervals.WEBSOCKET_LOAD_BALANCING_INDEX_DYNAMIC_CACHED_FILE_UPDATE)
        {
            RequestPath = requestPath;
            _ContentType = MimeTypes.MimeTypeMap.GetMimeType(filePath);
            List<string> placeholders = new List<string>(4) { GlobalConstants
                .Placeholders.WEBSOCKET_LOAD_BALANCING_INDEX_DYNAMIC_CACHED_FILE_PLACEHOLDER };
            if (additionalPlaceholderAndGetValue_s != null) {
                foreach (var a in additionalPlaceholderAndGetValue_s)
                {
                    placeholders.Add(a.Item1);
                }
            }
            _IndexFileWithInsert = new IndexFileWithInserts(filePath, placeholders.ToArray());
            _GetBytes = Get_GetBytes(additionalPlaceholderAndGetValue_s);
            _DefaultBytes = _GetBytes(defaultNodeId);
            _SequenceToHandOutForProportion = new byte[][] { _DefaultBytes };
        }
        private Func<int, byte[]> Get_GetBytes(Tuple<string, Func<string>>[] additionalPlaceholderAndGetValue_s) {
            if (additionalPlaceholderAndGetValue_s == null || additionalPlaceholderAndGetValue_s.Length < 1) {
                return (nodeId) => _IndexFileWithInsert.GetBytes(GetDomainForNodeId(nodeId));
            }
            return (nodeId) =>
            {
                string[] values = new string[additionalPlaceholderAndGetValue_s.Length + 1];
                values[0] = GetDomainForNodeId(nodeId);
                for (int i = 0; i < additionalPlaceholderAndGetValue_s.Length; i++)
                {
                    values[i + 1] = additionalPlaceholderAndGetValue_s[i].Item2();
                }
                return _IndexFileWithInsert.GetBytes(values);
            };
        }
        protected override void NewNodeIdsEstablished(int[] nodeIds)
        {
            byte[][] sequenceToHandOutForProportion = nodeIds
                        .Select(n => GetBytesForWebSocketServerNodeId(n))
                        .Where(b => b != null)
                        .ToArray();
                if (sequenceToHandOutForProportion.Length < 1)
                    sequenceToHandOutForProportion = new byte[][] { _DefaultBytes };
                lock (_LockObjectSequenceToHandleOutForProportion)
                {
                    _SequenceToHandOutForProportion = sequenceToHandOutForProportion;
                }
        }
        private byte[] GetBytesForWebSocketServerNodeId(int nodeId) {
            if (_MapNodeIdToBytes.TryGetValue(nodeId, out byte[] bytes))
                return bytes;
            bytes = _GetBytes(nodeId);
            _MapNodeIdToBytes[nodeId] = bytes;
            return bytes;
        }
        private string GetDomainForNodeId(int nodeId) {
            string[] domains = GlobalConstants.Nodes.UniqueDomainsForNode(nodeId);
            string domain =  domains.Where(d=>d.ToLower().IndexOf("ws")==0).FirstOrDefault();
            return domain;
        }
        public byte[] GetBytes(out string contentType)
        {
            contentType = _ContentType;
            lock (_LockObjectSequenceToHandleOutForProportion)
            {
                if (_CurrentIndex >= _SequenceToHandOutForProportion.Length)
                    _CurrentIndex = 0;
                return _SequenceToHandOutForProportion[_CurrentIndex++];
            }
        }
        public void Dispose()
        {
            _TimerScheduledUpdate.Dispose();
        }
    }
}