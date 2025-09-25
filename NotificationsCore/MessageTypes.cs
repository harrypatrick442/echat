using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace NotificationsCore
{
    public class MessageTypes
    {
        public const string
            UserNotificationsClear = InterserverMessageTypes.UserNotificationsClear,
            UserNotificationsUpdate = InterserverMessageTypes.UserNotificationsUpdate;
    }
}