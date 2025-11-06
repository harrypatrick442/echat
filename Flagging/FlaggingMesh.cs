using Core.Exceptions;
using Core.Ids;
using Flagging.Messages.Requests;
using Flagging.Messages.Responses;
using Initialization.Exceptions;
using InterserverComs;
using Logging;
using Shutdown;

namespace Flagging
{
    public partial class FlaggingMesh
    {
        private static FlaggingMesh _Instance;
        public static FlaggingMesh Initialize(int flaggingNodeId, int flaggingBackupNodeId) {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(FlaggingMesh));
            _Instance = new FlaggingMesh(flaggingNodeId, flaggingBackupNodeId);
            return _Instance;
        } 
        public static FlaggingMesh Instance { 
            get { 
                if (_Instance == null) throw new NotInitializedException(nameof(FlaggingMesh));
                return _Instance;
            } 
        }
        private long _MyNodeId;
        private int[] _NodeIds;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private FlaggingMesh(int flaggingNodeId, int flaggingBackupNodeId) {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _NodeIds = new int[] { flaggingNodeId, flaggingBackupNodeId};
            if (flaggingNodeId == Nodes.Nodes.Instance.MyId) {
                Initialize_Server();
            }
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.Flagging);
        }
        #region Methods
        #region Public
        public bool Flag(FlagRequest flagRequest)
        {

            bool successful = false;
            foreach (int nodeId in _NodeIds)
            {
                try
                {
                    OperationRedirectHelper.OperationRedirectedToNode<FlagRequest,
                        FlagResponse>(
                        nodeId,
                        () =>
                        {
                            successful = Flag_Here(flagRequest);
                        },
                        () => flagRequest,
                        (response) =>
                        {
                            successful = response.Success || successful;
                        },
                        _CancellationTokenSourceDisposed.Token
                    );
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
                if (successful) break;
            }
            return successful;
        }
        #endregion Public
        #region Private
        private void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
        }

        #endregion Private
#endregion Methods
    }
}