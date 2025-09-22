using Chat.Endpoints;
using Core.Interfaces;
using System.Net;

namespace EChatEndpoints.WebsocketServers
{
    public interface IUserChatClientEndpoint:IClientEndpoint
    {
        public ChatClientEndpoint ChatClientEndpoint { get; }
    }
}