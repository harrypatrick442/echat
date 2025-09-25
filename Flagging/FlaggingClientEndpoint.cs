using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using Core.Messages.Responses;
using JSON;
using Logging;
using Core.Interfaces;
using Core;
using Flagging.Messages.Requests;
using Flagging;
using Flagging.Messages.Responses;

namespace Flagging.Endpoints
{
    public class FlaggingClientEndpoint
    {
        private IClientEndpoint _ClientEndpoint;
        private long _MyUserId { get { return _ClientEndpoint.UserId; } }
        private Action _RemoveClientMessageTypeMappings;
        public FlaggingClientEndpoint(
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpoint clientEndpoint)
        {
            _ClientEndpoint = clientEndpoint;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { MessageTypes.FlaggingFlag, Flag }
            });
        }
        private void Flag(TypeTicketedAndWholePayload message)
        {
            try
            {
                FlagRequest request = Json.Deserialize<FlagRequest>(message.JsonString);
                request.UserIdFlagging = _MyUserId;
                long ticket = request.Ticket;
                bool success = FlaggingMesh.Instance.Flag(request);
                _ClientEndpoint.SendObject(new FlagResponse(success, ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        public void Dispose() {
            _RemoveClientMessageTypeMappings();
        }
    }
}