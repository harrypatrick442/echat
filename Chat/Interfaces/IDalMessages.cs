using Chat.Messages.Client;
using Chat.Messages.Client.Messages;
using MultimediaCore;

namespace Chat.Interfaces
{
    public interface IDalMessages
    {
        void Append(long conversationId, ClientMessage message, out ClientMessage replyMessage);
        void AddUserMultimediaItems(long conversationId, long messageId, UserMultimediaItem[] userMultimediaItems);
        void AddReaction(long conversationId, MessageReaction reaction);
        long[] Delete(long myUserId, long conversationId, long[] messageIds, out List<string> multimediaTokensDeleted);
        long[] DeleteAny(long conversationId, long[] messageIds, out List<string> multimediaTokensDeleted);
        void Modify(long conversationId, ClientMessage message, out List<string> multimediaTokensDeleted);
        ClientMessage[] ReadFromEnd(long conversationId, int nMessages, out MessageReaction[]? reactions,
            out MessageUserMultimediaItem[]? messageUserMultimediaItemss, 
            MessageChildConversationOptions messageChildConversationOptions);
        ClientMessage[] ReadRange(long conversationId, int? nMessages,
            long? idFromInclusive, long? idToExclusive, out MessageReaction[]? reactions,
            out MessageUserMultimediaItem[]? messageUserMultimediaItemss,
            MessageChildConversationOptions messageChildConversationOptions);
        ClientMessage[] ReadIndividualMessages(long conversationId, long[] messageIds);
        void RemoveReaction(long conversationId, MessageReaction reaction);
        void SetChildConversationIdForMessage(long parentConversationId, long messageId, long conversationId);
        long? GetChildConversationIdForMessage(long parentConversationId, long messageId);
        string[] GetTagsFromExistingMessage(long conversationId, long messageId);
    }
}