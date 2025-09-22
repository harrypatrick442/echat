using MentionsCore.Messages;
using MentionsCore.Requests;
using NotificationsCore.Enums;
using UserRoutedMessages;

namespace MentionsCore
{
    public sealed partial class MentionsMesh
    {
        private Mention[] Get_Here(long userId, int maxNEntries, long? toIdExclusive, long? fromIdInclusive)
        {
            return _DalMentionsSQLite.Get(userId, maxNEntries, toIdExclusive, fromIdInclusive);
        }
        private void Add_Here(long[] userIdBeingMentioneds, Mention mention, bool deleteExisting)
        {
            foreach (long userIdBeingMentioned in userIdBeingMentioneds) {
                NotificationsCore.UserNotificationsMesh.Instance.SetHasAt(
                    userIdBeingMentioned, NotificationType.Mentions, mention.AtTime);
            }
            _DalMentionsSQLite.Add(userIdBeingMentioneds, mention, deleteExisting);
        }
        private void SetSeen_Here(long userIdBeingMentioneds, long messageId) {
            _DalMentionsSQLite.SetSeen(userIdBeingMentioneds, messageId);
            UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                new SetSeenMention(userIdBeingMentioneds, messageId),
                userIdBeingMentioneds
            );
        }
    }
}