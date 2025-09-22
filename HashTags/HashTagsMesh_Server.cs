using JSON;
using Core.Handlers;
using Logging;
using InterserverComs;
using Core.Messages.Responses;
using MessageTypes.Internal;
using HashTags.Messages;
namespace HashTags
{
    public partial class HashTagsMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new Core.TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.SearchTags, HandleSearchTags},
                    {InterserverMessageTypes.AddTags, HandleAddTags},
                    {InterserverMessageTypes.DeleteTags, HandleDeleteTags},
                }
            );
        }
        private void HandleSearchTags(InterserverMessageEventArgs e)
        {
            SearchTagsRequest request = e.Deserialize<SearchTagsRequest>();
            SearchTagsResponse response;
            try
            {
                ScopeIds[]? exactMatches = SearchTags_Here(request.Tag, request.ScopeType, request.AllowPartialMatches, request.MaxNEntries, out TagWithScopeIds[]? partialMatches);
                response = new SearchTagsResponse(true, exactMatches, partialMatches, request.Ticket);
                
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SearchTagsResponse(false, null, null, request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleAddTags(InterserverMessageEventArgs e)
        {
            AddTagsRequest request = e.Deserialize<AddTagsRequest>();
            SuccessTicketedResponse response;
            try
            {
               AddTags_Here(request.Tags, request.ScopeType, request.ScopeId, request.ScopeId2);
                response = new SuccessTicketedResponse(true, request.Ticket);

            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleDeleteTags(InterserverMessageEventArgs e)
        {
            DeleteTagsRequest request = e.Deserialize<DeleteTagsRequest>();
            SuccessTicketedResponse response;
            try
            {
                DeleteTags_Here(request.ScopeType, request.ScopeId, request.ScopeId2, request.Tags);
                response = new SuccessTicketedResponse(true, request.Ticket);

            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}