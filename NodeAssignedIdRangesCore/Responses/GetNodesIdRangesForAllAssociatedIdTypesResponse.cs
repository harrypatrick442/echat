using Core.DataMemberNames;
using Core.Messages.Messages;
using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Responses;
using NodeAssignedIdRangesCore.Requests;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesCore.Responses
{
    [DataContract]
    public sealed class GetNodesIdRangesForAllAssociatedIdTypesResponse : TicketedMessageBase
    {
        //CHECKED
        [DataMember(Name = GetNodesIdRangesForAllAssociatedIdTypesResponseDataMemberNames
            .NodesIdRangesForIdTypes)]
        public NodesIdRangesForIdType[]? NodesIdRangesForIdTypes { get; protected set; }
        [JsonPropertyName(GetNodesIdRangesForAllAssociatedIdTypesResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = GetNodesIdRangesForAllAssociatedIdTypesResponseDataMemberNames.Success)]
        public bool Success{ get; protected set; }
        private GetNodesIdRangesForAllAssociatedIdTypesResponse(
            bool success,
            NodesIdRangesForIdType[]? nodesIdRangesForIdTypes,
            long ticket
        ):base(TicketedMessageType.Ticketed)
        {
            Success = success;
            NodesIdRangesForIdTypes = nodesIdRangesForIdTypes;
            Ticket = ticket;
        }
        protected GetNodesIdRangesForAllAssociatedIdTypesResponse() : base(TicketedMessageType.Ticketed) { }
        public static GetNodesIdRangesForAllAssociatedIdTypesResponse Successful(
            NodesIdRangesForIdType[] nodesIdRangesForIdTypes,
            long ticket)
        {
            return new GetNodesIdRangesForAllAssociatedIdTypesResponse(
                true, nodesIdRangesForIdTypes, ticket);
        }
        public static GetNodesIdRangesForAllAssociatedIdTypesResponse Failed(
            long ticket)
        {
            return new GetNodesIdRangesForAllAssociatedIdTypesResponse(
                false, null, ticket);
        }
    }
}
