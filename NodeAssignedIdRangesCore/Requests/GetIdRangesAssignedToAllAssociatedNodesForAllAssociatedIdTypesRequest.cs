using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;

namespace NodeAssignedIdRangesCore.Requests
{
    [DataContract]
    public class GetNodesIdRangesForAllAssociatedIdTypesRequest : TicketedMessageBase
    {
        public GetNodesIdRangesForAllAssociatedIdTypesRequest() 
            :base(InterserverMessageTypes.IdGetNodesIdRangesForAllAssociatedIdTypes)
        {
        }
    }
}
