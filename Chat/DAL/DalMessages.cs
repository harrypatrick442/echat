using Core.Exceptions;
using Chat;
using Chat.Interfaces;
using Initialization.Exceptions;
namespace Core.DAL
{
    public class DalMessages
    {
        private static  Dictionary<ConversationType, IDalMessages> _MapConversationTypeToDalMessages;
        public static void Initialize()
        {
            if (_MapConversationTypeToDalMessages != null)
                throw new AlreadyInitializedException(nameof(DalMessages));
            _MapConversationTypeToDalMessages = new Dictionary<ConversationType, IDalMessages> {
                {ConversationType.PublicChatroom, DalMessagesPublicChatrooms.Initialize()},
                {ConversationType.GroupChat, DalMessagesGroupChats.Initialize()},
                {ConversationType.Pm, DalMessagesPms.Initialize()},
                {ConversationType.Wall, DalMessagesWalls.Initialize()},
                {ConversationType.Comments, DalMessagesComments.Initialize()}
            };
        }
        public static IDalMessages ForConversationType(ConversationType conversationType) {
            return _MapConversationTypeToDalMessages[conversationType];
        }
    }
}