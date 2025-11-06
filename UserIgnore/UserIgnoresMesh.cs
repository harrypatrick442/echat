using Core.Exceptions;
using Shutdown;
using InterserverComs;
using Logging;
using NodeAssignedIdRangesCore.Requests;
using NodeAssignedIdRangesCore.Responses;
using Core.Handlers;
using Core;
using MessageTypes.Internal;
using UserIgnore.Requests;
using UserIgnore.Responses;
using UserRoutedMessages;
using UserIgnore.Messages;
using Users;
using Core.Ids;
using Core.DTOs;
using Initialization.Exceptions;
namespace UserIgnore
{
    public sealed partial class UserIgnoresMesh
    {
        private static UserIgnoresMesh? _Instance;
        public static UserIgnoresMesh Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(UserIgnoresMesh));
            _Instance = new UserIgnoresMesh();
            return _Instance;
        }
        public static UserIgnoresMesh Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(UserIgnoresMesh));
                return _Instance;
            }
        }
        private int _MyNodeId;
        private IIdentifierToNodeId<long> _UserIdToNodeId;
        private UserIgnoresMesh()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _UserIdToNodeId = UserIdToNodeId.Instance;
            Initialize_Server();
        }
        #region Methods
        #region Public
        public bool GetUserIgnores(long userId, out UserIgnores? userIgnores)
        {
            UserIgnores? userIgnoresInternal = null;
            bool success = true;
            OperationRedirectHelper.OperationRedirectedToNode<
                GetUserIgnoresRequest,
                GetUserIgnoresResponse>(
                _UserIdToNodeId.GetNodeIdFromIdentifier(userId),
                () => {
                    userIgnoresInternal = GetUserIgnores_Here(userId);
                },
                () => new GetUserIgnoresRequest(userId),
                (response) => {
                    userIgnoresInternal = response.UserIgnores;
                    success = response.Success;
                },
                ShutdownManager.Instance.CancellationToken
                );
            userIgnores = userIgnoresInternal;
            return success;
        }
        public bool AddUserIgnore(long userIdIgnoring, long userIdBeingIgnored)
        {
            if (!AddBeingIgnoredBy(userIdIgnoring, userIdBeingIgnored))
                return false;
            bool success = true;
            OperationRedirectHelper.OperationRedirectedToNode<
                AddUserIgnoreRequest,
                UserIgnoreSuccessResponse>(
                _UserIdToNodeId.GetNodeIdFromIdentifier(userIdIgnoring),
                () => {
                    AddUserIgnore_Here(userIdIgnoring, userIdBeingIgnored);
                },
                () => new AddUserIgnoreRequest(userIdIgnoring, userIdBeingIgnored),
                (response) => {
                    success = response.Success;
                },
                ShutdownManager.Instance.CancellationToken
                );
            if (success) {
                UserRoutedMessagesManager.Instance
                    .ForwardObjectToUserDevices(new IgnoredUser(userIdBeingIgnored), userIdIgnoring);
            }
            return success;
        }
        public bool RemoveUserIgnore(long userIdUnignoring, long userIdBeingUnignored)
        {
            bool success = true;
            OperationRedirectHelper.OperationRedirectedToNode<
                RemoveUserIgnoreRequest,
                UserIgnoreSuccessResponse>(
                _UserIdToNodeId.GetNodeIdFromIdentifier(userIdUnignoring),
                () => {
                    RemoveUserIgnore_Here(userIdUnignoring, userIdBeingUnignored);
                },
                () => new RemoveUserIgnoreRequest(userIdUnignoring, userIdBeingUnignored),
                (response) => {
                    success = response.Success;
                },
                ShutdownManager.Instance.CancellationToken
                );
            if (!success) return false;
            success = RemoveBeingIgnoredBy(userIdUnignoring, userIdBeingUnignored);
            if (success)
            {
                UserRoutedMessagesManager.Instance
                    .ForwardObjectToUserDevices(new UnignoredUser(userIdBeingUnignored), userIdUnignoring);
            }
            return success;
        }
        #endregion Public
        #region Private
        private bool AddBeingIgnoredBy(long userIdIgnoring, long userIdBeingIgnored)
        {
            bool success = true;
            OperationRedirectHelper.OperationRedirectedToNode<
                AddBeingIgnoredByRequest,
                UserIgnoreSuccessResponse>(
                _UserIdToNodeId.GetNodeIdFromIdentifier(userIdBeingIgnored),
                () => {
                    AddBeingIgnoredBy_Here(userIdIgnoring, userIdBeingIgnored);
                },
                () => new AddBeingIgnoredByRequest(userIdIgnoring, userIdBeingIgnored),
                (response) => {
                    success = response.Success;
                },
                ShutdownManager.Instance.CancellationToken
                );
            return success;
        }
        private bool RemoveBeingIgnoredBy(long userIdUnignoring, long userIdBeingUnignored)
        {
            bool success = true;
            OperationRedirectHelper.OperationRedirectedToNode<
                RemoveBeingIgnoredByRequest,
                UserIgnoreSuccessResponse>(
                _UserIdToNodeId.GetNodeIdFromIdentifier(userIdBeingUnignored),
                () => {
                    RemoveBeingIgnoredBy_Here(userIdUnignoring, userIdBeingUnignored);
                },
                () => new RemoveBeingIgnoredByRequest(userIdUnignoring, userIdBeingUnignored),
                (response) => {
                    success = response.Success;
                },
                ShutdownManager.Instance.CancellationToken
                );
            return success;
        }
        #endregion Private
        #endregion Methods
    }
}