using JSON;
using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Core;
using Core.Machine;
using Sessions.Requests;
using Sessions.Responses;
namespace Sessions
{
    public partial class SessionsMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.SessionsAuthenticate, HandleAuthenticate}
                }
            );
        }
        private void HandleAuthenticate(InterserverMessageEventArgs e)
        {
            SessionsGetTokenRequest request = e.Deserialize<SessionsGetTokenRequest>();
            try
            {
                long? userId = Authenticate_Here(request.SessionId, request.Token);
                SessionsGetTokenResponse response = new SessionsGetTokenResponse(userId, request.Ticket);
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}