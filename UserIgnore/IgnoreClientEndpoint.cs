using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using Core.Messages.Responses;
using JSON;
using Core.Interfaces;
using Core;
using UserIgnore;
using UserIgnore.Requests;

namespace UserIgnore
{
    public class IgnoreClientEndpoint
    {
        private IClientEndpoint _Endpoint;
        private long _MyUserId { get { return _Endpoint.UserId; } }
        private Action _RemoveClientMessageTypeMappings;
        public IgnoreClientEndpoint(
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpoint snippetsClientEndpoint)
        {
            _Endpoint = snippetsClientEndpoint;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { MessageTypes.UserIgnoreIgnore, HandleIgnoreUser},
                { MessageTypes.UserIgnoreUnignore, HandleUnignoreUser},
                { MessageTypes.UserIgnoreGet, HandleGetUserIgnores}
            });
        }
        private void HandleGetUserIgnores(TypeTicketedAndWholePayload message)
        {
            if (!_Endpoint.HasSession) return;
            GetUserIgnoresRequest request = Json.Deserialize<GetUserIgnoresRequest>(message.JsonString);
            bool success = UserIgnoresMesh.Instance.GetUserIgnores(
                _MyUserId, out UserIgnores? userIgnores);
            _Endpoint.SendObject(new GetUserIgnoresResponse(success, userIgnores, request.Ticket));
        }
        private void HandleIgnoreUser(TypeTicketedAndWholePayload message)
        {
            if (!_Endpoint.HasSession) return;
            IgnoreUserRequest request = Json.Deserialize<IgnoreUserRequest>(message.JsonString);
            bool success = UserIgnoresMesh.Instance.AddUserIgnore(_MyUserId, request.UserId);
            _Endpoint.SendObject(new SuccessTicketedResponse(success, request.Ticket));
        }
        private void HandleUnignoreUser(TypeTicketedAndWholePayload message)
        {
            if (!_Endpoint.HasSession) return;
            UnignoreUserRequest request = Json.Deserialize<UnignoreUserRequest>(message.JsonString);
            bool success = UserIgnoresMesh.Instance.RemoveUserIgnore(_MyUserId, request.UserId);
            _Endpoint.SendObject(new SuccessTicketedResponse(success, request.Ticket));
        }
        public void Dispose() {
            _RemoveClientMessageTypeMappings();
        }
    }
}