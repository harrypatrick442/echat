using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using Core.Messages.Responses;
using JSON;
using Core.Interfaces;
using Core;
using MentionsCore;
using MentionsCore.Responses;
using MentionsCore.Requests;
using MentionsCore.Messages;

namespace MentionsCore
{
    public class MentionsClientEndpoint
    {
        private IClientEndpoint _Endpoint;
        private long _MyUserId { get { return _Endpoint.UserId; } }
        private Action _RemoveClientMessageTypeMappings;
        public MentionsClientEndpoint(
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpoint snippetsClientEndpoint)
        {
            _Endpoint = snippetsClientEndpoint;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { MessageTypes.MentionsGet, HandleGetMentions},
                { MessageTypes.MentionsSetSeen, HandleSetSeenMention}
            });
        }
        private void HandleGetMentions(TypeTicketedAndWholePayload message)
        {
            if (!_Endpoint.HasSession) return;
            GetMentionsRequest request = Json.Deserialize<GetMentionsRequest>(message.JsonString);
            bool success = MentionsMesh.Instance.Get(
                _MyUserId, request.NEntries, out Mention[]? mentions, request.IdToExclusive, request.IdFromInclusive);
            GetMentionsResponse response = success
                ? GetMentionsResponse.Success(mentions!, request.Ticket)
                : GetMentionsResponse.Failed(request.Ticket);
            _Endpoint.SendObject(response);
        }
        private void HandleSetSeenMention(TypeTicketedAndWholePayload message)
        {
            if (!_Endpoint.HasSession) return;
            SetSeenMention request = Json.Deserialize<SetSeenMention>(message.JsonString);
            MentionsMesh.Instance.SetSeen(_MyUserId, request.MessageId);
        }
        public void Dispose() {
            _RemoveClientMessageTypeMappings();
        }
    }
}