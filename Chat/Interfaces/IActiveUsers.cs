using Chat.Messages.Client;
using Chat.Messages.Client.Messages;
using Core.Chat;
using MultimediaCore;

namespace Chat.Interfaces
{
    public interface IActiveUsers: IConversation
    {
        public void AddActiveUser(long userId);
        public void RemoveActiveUser(long userId);
    }
}