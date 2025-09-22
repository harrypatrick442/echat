using InterserverComs;
using JSON;
using Logging;
using NodeAssignedIdRangesCore.Requests;
using NodeAssignedIdRangesCore.Responses;

namespace NodeAssignedIdRanges
{
    public sealed partial class IdRangesMesh
    {
        //CHECKED
        private const int MAX_N_ATTEMPTS_SEND_TO_NODE = 2;
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        private void HandleGiveMeNewIdRange(InterserverMessageEventArgs e)
        {
            GiveMeNewIdRangeRequest request = e.Deserialize<GiveMeNewIdRangeRequest>();
            GiveMeNewIdRangeResponse response;
            try
            {
                int nodeId = e.EndpointFrom.NodeId;
                IdRange newIdRange = GiveMeNewIdRange_Here(request.IdType, nodeId);
                response = new GiveMeNewIdRangeResponse(newIdRange, request.Ticket);
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.HighPriority.Error(ex);
            }
        }
        private void HandleAnotherServerGotANewIdRange(InterserverMessageEventArgs e)
        {
            AnotherServerGotANewIdRangeRequest request = e.Deserialize<AnotherServerGotANewIdRangeRequest>();
            AcknowledgeResponse response;
            try
            {
                int nodeId = e.EndpointFrom.NodeId;
                AnotherServerGotANewIdRange_Here(request.IdType, nodeId, request.NodeIdRange);
                response = new AcknowledgeResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                response = new AcknowledgeResponse(false, request.Ticket);
                Logs.HighPriority.Error(ex);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.HighPriority.Error(ex);
            }
        }
        public void HandleGetNodesIdRangesForAllAssociatedIdTypes(InterserverMessageEventArgs e)
        {
            GetNodesIdRangesForAllAssociatedIdTypesRequest request = e.Deserialize<GetNodesIdRangesForAllAssociatedIdTypesRequest>();
            GetNodesIdRangesForAllAssociatedIdTypesResponse response;
            try
            {
                NodesIdRangesForIdType[] nodesIdRangesForIdTypes =
                    GetNodesIdRangesForAllAssociatedIdTypes_Here(e.EndpointFrom.NodeId);
                response = GetNodesIdRangesForAllAssociatedIdTypesResponse.Successful(nodesIdRangesForIdTypes, request.Ticket);
            }
            catch (Exception ex)
            {
                response = GetNodesIdRangesForAllAssociatedIdTypesResponse.Failed(request.Ticket);
                Logs.HighPriority.Error(ex);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.HighPriority.Error(ex);
            }
        }
    }
}