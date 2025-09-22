using JSON;
using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Core;
using UserIgnore.Requests;
using UserIgnore.Responses;

namespace UserIgnore
{
    public sealed partial class UserIgnoresMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        private void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    { InterserverMessageTypes.UserIgnoresGet, HandleGetUserIgnores},
                    { InterserverMessageTypes.UserIgnoresAdd,HandleAddUserIgnore},
                    { InterserverMessageTypes.UserIgnoresRemove,HandleRemoveUserIgnore},
                    { InterserverMessageTypes.UserIgnoresAddBeingIgnoredBy,HandleAddBeingIgnoredBy},
                    { InterserverMessageTypes.UserIgnoresRemoveBeingIgnoredBy,HandleRemoveBeingIgnoredBy}
                }
            );
        }
        private void HandleGetUserIgnores(InterserverMessageEventArgs e)
        {
            GetUserIgnoresRequest request = e.Deserialize<GetUserIgnoresRequest>();
            GetUserIgnoresResponse response;
            try
            {
                UserIgnores userIgnores = GetUserIgnores_Here(request.UserId);
                response = new GetUserIgnoresResponse(true, userIgnores, request.Ticket);
            }
            catch(Exception ex)
            {
                Logs.Default.Error(ex);
                response = new GetUserIgnoresResponse(false, null, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleAddUserIgnore(InterserverMessageEventArgs e)
        {
            AddUserIgnoreRequest request = e.Deserialize<AddUserIgnoreRequest>();
            UserIgnoreSuccessResponse response;
            try
            {
                AddUserIgnore_Here(request.UserIdIgnoring, request.UserIdBeingIgnored);
                response = new UserIgnoreSuccessResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new UserIgnoreSuccessResponse(false, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleRemoveUserIgnore(InterserverMessageEventArgs e)
        {
            RemoveUserIgnoreRequest request = e.Deserialize<RemoveUserIgnoreRequest>();
            UserIgnoreSuccessResponse response;
            try
            {
                RemoveUserIgnore_Here(request.UserIdUnignoring, request.UserIdBeingUnignored);
                response = new UserIgnoreSuccessResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new UserIgnoreSuccessResponse(false, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleAddBeingIgnoredBy(InterserverMessageEventArgs e)
        {
            AddBeingIgnoredByRequest request = e.Deserialize<AddBeingIgnoredByRequest>();
            UserIgnoreSuccessResponse response;
            try
            {
                AddBeingIgnoredBy_Here(request.UserIdIgnoring, request.UserIdBeingIgnored);
                response = new UserIgnoreSuccessResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new UserIgnoreSuccessResponse(false, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleRemoveBeingIgnoredBy(InterserverMessageEventArgs e)
        {
            RemoveBeingIgnoredByRequest request = e.Deserialize<RemoveBeingIgnoredByRequest>();
            UserIgnoreSuccessResponse response;
            try
            {
                RemoveBeingIgnoredBy_Here(request.UserIdUnignoring, request.UserIdBeingUnignored);
                response = new UserIgnoreSuccessResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new UserIgnoreSuccessResponse(false, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
    }
}