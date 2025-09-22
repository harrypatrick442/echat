using Logging;
using JSON;
using Core.Threading;
using Nodes;
using InterserverComs;
using Core.Interfaces;
using NodeAssignedIdRanges;
using Database;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using DataMemberNames.Client;

namespace HashTags.Messages
{
    [DataContract]
    public class AddTagsResponse:TicketedMessageBase
    {
        [JsonPropertyName(AddTagsResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = AddTagsResponseDataMemberNames.Success)]
        public bool Success  { get; protected set; }
        public AddTagsResponse(bool success, long ticket)
            :base(ClientMessageTypes.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
    }
}
