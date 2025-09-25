using Core;
using Core.Handlers;
using InterserverComs;
using JSON;
using Location.Requests;
using Location.Responses;
using LocationCore;
using Logging;
using System;

namespace Location
{
    public partial class QuadTreeMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    { InterserverMessageTypes.QuadTreeSetOnIdAssociatedNode , HandleSetOnIdAssociatedNode},
                    { InterserverMessageTypes.QuadTreeSetSpecificToNode , HandleSetSpecificToNode},
                    { InterserverMessageTypes.QuadTreeDeleteOnIdAssociatedNode, HandleDeleteOnIdAssociatedNode},
                    { InterserverMessageTypes.QuadTreeDeleteSpecificToNode, HandleDeleteSpecificToNode},
                    { InterserverMessageTypes.QuadTreeGetIdsSpecificToNode, HandleGetIdsSpecificToNode},
                    { InterserverMessageTypes.QuadTreeGetNEntriesSpecificToNode, HandleGetNEntriesSpecificToNode}
                }
            );
        }
        private void HandleSetOnIdAssociatedNode(InterserverMessageEventArgs e) {
            SetOnIdAssociatedNodeRequest request = e.Deserialize<SetOnIdAssociatedNodeRequest>();
            SetOnIdAssociatedNodeResponse response;
            try
            {
                SetOnIdAssociatedNode_Here(request.DatabaseIdentifier, 
                    request.Id, request.LatLng);
                response = SetOnIdAssociatedNodeResponse.Success(request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = SetOnIdAssociatedNodeResponse.Failure(request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleSetSpecificToNode(InterserverMessageEventArgs e)
        {
            SetSpecificToNodeRequest request = e.Deserialize<SetSpecificToNodeRequest>();
            SetSpecificToNodeResponse response;
            try
            {
                SetSpecificToNode_Here(request.DatabaseIdentifier, request.Id, request.LatLng, request.LevelQuadrantPairs);
                response = SetSpecificToNodeResponse.Success(request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = SetSpecificToNodeResponse.Failure(request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleDeleteOnIdAssociatedNode(InterserverMessageEventArgs e)
        {
            DeleteOnIdAssociatedNodeRequest request = e.Deserialize<DeleteOnIdAssociatedNodeRequest>();
            DeleteOnIdAssociatedNodeResponse response;
            try
            {
                DeleteOnIdAssociatedNode_Here(request.DatabaseIdentifier, request.Id);
                response = DeleteOnIdAssociatedNodeResponse.Success(request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = DeleteOnIdAssociatedNodeResponse.Failure(request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleDeleteSpecificToNode(InterserverMessageEventArgs e)
        {
            DeleteSpecificToNodeRequest request = e.Deserialize<DeleteSpecificToNodeRequest>();
            DeleteSpecificToNodeResponse response;
            try
            {
                DeleteSpecificToNode_Here(request.DatabaseIdentifier, request.Id, request.Levels);
                response = DeleteSpecificToNodeResponse.Success(request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = DeleteSpecificToNodeResponse.Failure(request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }

        private void HandleGetIdsSpecificToNode(InterserverMessageEventArgs e)
        {
            GetIdsSpecificToNodeRequest request = e.Deserialize<GetIdsSpecificToNodeRequest>();
            GetIdsSpecificToNodeResponse response;
            try
            {
                Quadrant[] quadrants = GetIdsSpecificToNode_Here(request.DatabaseIdentifier, request.LevelQuadrantPairs);
                response = GetIdsSpecificToNodeResponse.Success(quadrants, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetIdsSpecificToNodeResponse.Failure(request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleGetNEntriesSpecificToNode(InterserverMessageEventArgs e)
        {
            GetNEntriesSpecificToNodeRequest request = e.Deserialize<GetNEntriesSpecificToNodeRequest>();
            GetNEntriesSpecificToNodeResponse response;
            try
            {
                QuadrantNEntries[] quadrantNEntriess= GetNEntriesSpecificToNode_Here(
                    request.DatabaseIdentifier, request.Level, request.Quadrants, request.WithLatLng);
                response = GetNEntriesSpecificToNodeResponse.Success(quadrantNEntriess, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetNEntriesSpecificToNodeResponse.Failure(request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
    }
}
