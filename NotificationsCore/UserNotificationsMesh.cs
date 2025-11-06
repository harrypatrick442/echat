using Core.Exceptions;
using Shutdown;
using InterserverComs;
using NotificationsCore.Messages.Messages;
using NotificationsCore.Messages;
using NotificationsCore.Messages.Requests;
using Chat.Messages.Client.Responses;
using Users;
using NotificationsCore.Enums;
using Logging;
using Initialization.Exceptions;

namespace NotificationsCore
{
    public partial class UserNotificationsMesh
    {
        private static UserNotificationsMesh? _Instance;
        public static UserNotificationsMesh Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(UserNotificationsMesh));
            _Instance = new UserNotificationsMesh();
            return _Instance;
        }
        public static UserNotificationsMesh Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(UserNotificationsMesh));
                return _Instance;
            }
        }
        private long _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private UserNotificationsMesh()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.MultimediaServerMesh);

        }
        #region Methods
        #region Public
        public UserNotifications? Get(long userId)
        {
            UserNotifications userNotifications = null;
            OperationRedirectHelper.OperationRedirectedToNode<
                GetUserNotificationsRequest, GetUserNotificationsResponse>(
                UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userId),
                () => userNotifications = GetUserNotifications_Here(userId),
                () => new GetUserNotificationsRequest(userId),
                (response) =>
                {
                    userNotifications = response.UserNotifications;
                },
                _CancellationTokenSourceDisposed.Token
            );
            return userNotifications;
        }
        public bool ClearUserNotification(long userId, NotificationType notificationType, long upToAtInclusive)
        {
            bool cleared = false;
            OperationRedirectHelper.OperationRedirectedToNode<
                ClearUserNotificationRequest, ClearUserNotificationResponse>(
                UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userId),
                () => cleared = ClearUserNotification_Here(userId, notificationType, upToAtInclusive),
                () => new ClearUserNotificationRequest(userId, notificationType, upToAtInclusive),
                (response) =>
                {
                    cleared = response.Cleared;
                },
                _CancellationTokenSourceDisposed.Token
            );
            return cleared;
        }
        public void SetHasAt(long userId, NotificationType notificationType, long at)
        {
            try
            {
                OperationRedirectHelper.OperationRedirectedToNode(
                    UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userId),
                    () => SetUserNotificationHasAt_Here(userId, notificationType, at),
                    () => new SetUserNotificationHasAtRequest(userId, notificationType, at)
                );
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
            }
        }
        #endregion Public
        #region Private
        private void Dispose()
        {
            _CancellationTokenSourceDisposed.Cancel();
        }
        #endregion
        #endregion Methods
    }
}