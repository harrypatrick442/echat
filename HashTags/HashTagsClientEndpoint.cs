using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using Core.Messages.Responses;
using JSON;
using Core.Interfaces;
using Core;
using HashTags.Messages;
using HashTags;
using Logging;

namespace HashTags
{
    public class HashTagsClientEndpoint
    {
        private IClientEndpoint _Endpoint;
        private Action _RemoveClientMessageTypeMappings;
        private const int MAX_N_PREDICTIONS = 40;
        public HashTagsClientEndpoint(
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpoint snippetsClientEndpoint)
        {
            _Endpoint = snippetsClientEndpoint;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { MessageTypes.SearchToPredictTag, HandleSearchToPredictTag},
                { MessageTypes.SearchTagsMultipleScopeTypes, HandleSearchTags}
            });
        }
        public void HandleSearchToPredictTag(TypeTicketedAndWholePayload m)
        {
            var request = Json.Deserialize<SearchToPredictTagRequest>(m.JsonString);
            SearchToPredictTagResponse response;
            try
            {
                bool success = HashTagsMesh.Instance.SearchToPredictTag(request.Str, request.ScopeType, MAX_N_PREDICTIONS, out string[] matches);
                response = new SearchToPredictTagResponse(success, matches, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SearchToPredictTagResponse(false, null, request.Ticket);
            }
            _Endpoint.SendObject(response);
        }
        public void HandleSearchTags(TypeTicketedAndWholePayload m)
        {
            var request = Json.Deserialize<SearchTagsMultipleScopeTypesRequest>(m.JsonString);
            SearchTagsMultipleScopeTypesResponse response 
                = new SearchTagsMultipleScopeTypesResponse(
                    HashTagsMesh.Instance.SearchTagsMultipleScopeTypes(
                        request.Tag,
                        request.ScopeTypes,
                        request.AllowPartialMatches,
                        request.MaxNEntriesPerScopeType
                    ),
                    m.Ticket);
            /*foreach (SearchTagsResultForScopeType resultForScopeType in response.Results) {
                switch (resultForScopeType.ScopeType) {
                    case IdTypes.MESSAGE:
                        break;
                    case IdTypes.CHAT_ROOM:
                        break;
                }
            }*/
            _Endpoint.SendObject(response);
        }
        public void Dispose() {
            _RemoveClientMessageTypeMappings();
        }
    }
}