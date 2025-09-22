using JSON;
using Chat.Interfaces;
using Core.DAL;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client;
using MultimediaServerCore;
using HashTags;
using GlobalConstants;

namespace Chat.MessagesHandler
{
    public class ChatRoomMessagesHandler_FullHistory : ChatRoomMessagesHandlerBase
    {
        private long _ConversationId;
        private IDalMessages _DalMessages;
        public ChatRoomMessagesHandler_FullHistory(ConversationType conversationType, long conversationId)
            :base(GlobalConstants.Lengths.MAX_N_MESSAGES_IN_OVERFLOWING_CHAT_ROOM)
        {
            _ConversationId = conversationId;
            _DalMessages = DalMessages.ForConversationType(conversationType);
            ClientMessage[] latestMessages = _DalMessages.ReadFromEnd(
                conversationId, GlobalConstants.Lengths.MAX_N_MESSAGES_IN_OVERFLOWING_CHAT_ROOM,
                out MessageReaction[] reactions, out MessageUserMultimediaItem[] messageUserMultimediaItems,
                null);
            _LatestCachedMessages.Initialize(latestMessages, reactions, messageUserMultimediaItems);
        }
        public override void Add(ClientMessage message, out string jsonString)
        {
            message.ConversationId = _ConversationId;
            message.ReplyMessage = null;
            _DalMessages.Append(_ConversationId, message, out ClientMessage replyMessage);
            message.ReplyMessage = replyMessage;
            MentionsHelper.SendMentionsToServersForMentionedUsers(message, isUpdate:false);
            jsonString = Json.Serialize(message);
            _LatestCachedMessages.Add(message, jsonString);
        }

        public override long[] DeleteMessages(long myUserId, long[] messageIds,
            bool canDeleteAnyMessage)
        {
            HashSet<long> deletedIds;
            List<string> multimediaTokensDeleted;
            if (canDeleteAnyMessage)
                deletedIds = _DalMessages.DeleteAny(
                    _ConversationId,
                    messageIds,
                    out multimediaTokensDeleted).ToHashSet();
            else
                deletedIds = _DalMessages.Delete(
                    myUserId,
                    _ConversationId,
                    messageIds,
                    out multimediaTokensDeleted
                ).ToHashSet();
            if(multimediaTokensDeleted!=null)
                MultimediaServerMesh.Instance.Delete(multimediaTokensDeleted);
            foreach (long deletedId in _LatestCachedMessages.Delete(
                    myUserId,
                    messageIds,
                    canDeleteAnyMessage)) {
                if (!deletedIds.Contains(deletedId))
                    deletedIds.Add(deletedId);
            }
            return deletedIds.ToArray();
        }

        public override bool ModifyMessage(ModifyMessage modifyMessageRequest)
        {
            ClientMessage message = modifyMessageRequest.Message;
            message.ConversationId = _ConversationId;
            _DalMessages.Modify(
                _ConversationId,
                message, out List<string> multimediaTokensDeleted);
            if (multimediaTokensDeleted != null)
            {
                MultimediaServerMesh.Instance.Delete(multimediaTokensDeleted);
            }
            bool success = _LatestCachedMessages.Modify(
                message);
            if (success)
            {
                MentionsHelper.SendMentionsToServersForMentionedUsers(message, isUpdate: true);
            }
            return success;
        }

        public override void ReactToMessage(ReactToMessage reactToMessage)
        {
            _DalMessages.AddReaction(_ConversationId, reactToMessage.MessageReaction);
            _LatestCachedMessages.ReactToMessage(reactToMessage.MessageReaction);
        }

        public override void UnreactToMessage(UnreactToMessage unreactToMessage)
        {
            _DalMessages.RemoveReaction(_ConversationId, unreactToMessage.MessageReaction);
            _LatestCachedMessages.UnreactToMessage(unreactToMessage.MessageReaction);
        }

        public override void LoadMessagesHistory(
            long? idFromInclusive, long? idToExclusive, int? nEntries, out ClientMessage[] messages,
            out MessageReaction[] reactions, out MessageUserMultimediaItem[] userMultimediaItems)
        {
            if (idFromInclusive == null && idToExclusive == null)
            {
                _LatestCachedMessages.GetNMessagesFromEnd(nEntries, out messages, out reactions, out userMultimediaItems);
                return;
            }
            messages = _DalMessages
                .ReadRange(_ConversationId, nEntries, idFromInclusive, idToExclusive, 
                out reactions, out userMultimediaItems, null);
        }
        public override void FlushMessagesIfChanged() { }
        protected override void FlushMessages(ClientMessage[] messages, MessageReaction[] reactions, MessageUserMultimediaItem[] userMultimediaItems){}
    }
}