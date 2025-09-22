using Core.Handlers;
using Flagging.Messages.Requests;
using Flagging.Messages.Responses;
using InterserverComs;
using JSON;
using Logging;
using MessageTypes.Internal;
namespace Flagging
{
    public partial class FlaggingMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new Core.TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.FlaggingFlag, HandleFlag}
                }
            );
        }
        private void HandleFlag(InterserverMessageEventArgs e)
        {
            FlagRequest request = e.Deserialize<FlagRequest>();
            try
            {
                FlagResponse response = new FlagResponse(
                    Flag_Here(request), request.Ticket);
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}