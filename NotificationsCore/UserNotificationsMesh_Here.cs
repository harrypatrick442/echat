using Core.DTOs;
using NotificationsCore.Enums;
using NotificationsCore.Messages;
using NotificationsCore.Messages.Messages;
using Org.BouncyCastle.Utilities.IO;
using UserRoutedMessages;

namespace NotificationsCore
{
    public partial class UserNotificationsMesh
    {
        private bool ClearUserNotification_Here(
            long userId,
            NotificationType notificationType,
            long upToAtInclusive)
        {
            bool cleared = DalNotifications.Instance.ClearUpToAtInclusive(userId, notificationType, upToAtInclusive);
            if (cleared)
            {
                PushUpdateToUserDevices(userId, notificationType, null);
            }
            return cleared;
        }
        private void SetUserNotificationHasAt_Here(
            long userId,
            NotificationType notificationType,
            long at)
        {
            DalNotifications.Instance.SetHasAt(userId, notificationType, at);
            //PushUpdateToUserDevices(userId, notificationType, at);
        }
        private UserNotifications GetUserNotifications_Here(
            long userId)
        {
            return DalNotifications.Instance.GetUserNotifications(userId);
        }
        private void PushUpdateToUserDevices(long userId,
            NotificationType notificationType, long? at)
        {
            UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                new UserNotificationUpdate(notificationType, at),
                userId
            );
            
        }
    }
}