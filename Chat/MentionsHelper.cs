using MentionsCore;
using MentionsCore.Messages;
using Core.Timing;
using Chat.Messages.Client.Messages;

namespace Chat
{
    public static class MentionsHelper
    {   
        public static void SendMentionsToServersForMentionedUsers(ClientMessage clientMessage, bool isUpdate)
        {
            if (clientMessage == null || clientMessage.MentionUserIds == null || clientMessage.MentionUserIds.Length < 1)
                return;
            Mention mention = new Mention(clientMessage.UserId, TimeHelper.MillisecondsNow,
                clientMessage.Id, clientMessage.ConversationId, clientMessage.Content, false);
            MentionsMesh.Instance.Add(clientMessage.MentionUserIds, mention, isUpdate);
        }
    }
}