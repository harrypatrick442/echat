using Core.Messages.Messages;
using MessageTypes.Internal;
using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Requests;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesCore.Requests
{
    [DataContract]
    public class GiveMeNewIdRangeRequest:TicketedMessageBase
    {
        //CHECKED
        [JsonPropertyName(GiveMeNewIdRangeRequestDataMemberNames.IdType)]
        [JsonInclude]
        [DataMember(Name = GiveMeNewIdRangeRequestDataMemberNames.IdType)]
        public int IdType { get; protected set; }
        public GiveMeNewIdRangeRequest(int idType) 
            :base(InterserverMessageTypes.IdsGiveMeNewIdRange){
            IdType = idType;
        }
        protected GiveMeNewIdRangeRequest()
            : base(InterserverMessageTypes.IdsGiveMeNewIdRange) { }
    }
}
