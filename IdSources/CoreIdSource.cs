using Core.Assets;
using Nodes;
using Core.Exceptions;
using NodeAssignedIdRanges;

namespace Core.Ids
{
    public sealed class CoreIdSource : NodeAssignedIdSource
    {
        private static CoreIdSource _Instance;
        public static CoreIdSource Instance { get { return _Instance; } }
        private NodeIdRanges _CurrentNodeIdRange, _CurrentNodeIdRangeSanityCheck;
        private INode _NodeMe;
        public static CoreIdSource Initialize(INode nodeMe) {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(CoreIdSource));
            _Instance = new CoreIdSource(nodeMe);
            return _Instance;
        }
        private CoreIdSource(INode nodeMe) 
            : base(nodeMe, Paths.SnippetsIdSourceDirectory)
        {

        }
    }
}