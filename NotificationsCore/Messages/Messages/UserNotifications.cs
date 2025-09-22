using Core.Timing;
using NotificationsCore.DataMemberNames.Messages;
using NotificationsCore.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace NotificationsCore.Messages.Messages
{
    [DataContract]
    public class UserNotifications
    {
        private const long OFFSET_FOR_ERROR_DETECTION = 24 * 60 * 60 * 1000;
        [JsonPropertyName(UserNotificationsDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = UserNotificationsDataMemberNames.Entries)]
        public UserNotification[]? Entries { get; protected set; }
        public UserNotifications(NotificationType notificationType, long hasAt)
        {
            Entries = new UserNotification[] { new UserNotification(notificationType, hasAt) };
        }
        protected UserNotifications()
        {

        }
        public bool ClearIfTimeGreater(NotificationType notificationType, long upToAtInclusive)
        {
            if (Entries == null) return true;
            UserNotification? entry = Entries
                .Where(e => e.NotificationType.Equals(notificationType))
                .FirstOrDefault();
            if (entry == null)
            {
                return true;
            }
            if (entry.At > upToAtInclusive)
            {
                if (entry.At < TimeHelper.MillisecondsNow + OFFSET_FOR_ERROR_DETECTION)
                {
                    return false;
                }
            }
            if (Entries.Length < 2)
            {
                Entries = null;
                return true;
            }
            Entries = Entries.Where(e => !e.Equals(entry)).ToArray();
            return true;
        }
        public void SetAt(NotificationType notificationType, long at)
        {
            if (Entries == null)
            {
                Entries = new UserNotification[] { new UserNotification(notificationType, at) };
                return;
            }
            UserNotification? existingEntry = Entries.Where(e => e.NotificationType.Equals(notificationType)).FirstOrDefault();
            if (existingEntry != null)
            {
                if (existingEntry.At > at)
                {
                    if (existingEntry.At < TimeHelper.MillisecondsNow + OFFSET_FOR_ERROR_DETECTION)
                    {
                        return;
                    }
                }
                existingEntry.At = at;
                return;
            }
            UserNotification[] oldEntries = Entries;
            Entries = new UserNotification[oldEntries.Length + 1];
            Array.Copy(oldEntries, Entries, oldEntries.Length);
            Entries[oldEntries.Length] = new UserNotification(notificationType, at);
        }
    }
}