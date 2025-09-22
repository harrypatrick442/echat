using Core.DataMemberNames;
using Core.Messages.Messages;
using LocationCore;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserLocation.DataMemberNames.Responses;

namespace UserLocation
{
    [DataContract]
    public class UserQuadTreeGetNEntriesResponse : TicketedMessageBase
    {
        [JsonPropertyName(UserQuadTreeGetNEntriesResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetNEntriesResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(UserQuadTreeGetNEntriesResponseDataMemberNames.QuadrantNEntriess)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetNEntriesResponseDataMemberNames.QuadrantNEntriess)]
        public QuadrantNEntries[] QuadrantNEntriess { get; protected set; }
        public UserQuadTreeGetNEntriesResponse(long ticket, bool success, 
            QuadrantNEntries[] quadrantNEntriess)
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet)
        {
            Success = success;
            QuadrantNEntriess = quadrantNEntriess;
            Ticket = ticket;
        }
        protected UserQuadTreeGetNEntriesResponse()
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet) { }
        public static UserQuadTreeGetNEntriesResponse Successful(QuadrantNEntries[] quadrantNEntriess, long ticket)
        {
            return new UserQuadTreeGetNEntriesResponse(ticket, true, quadrantNEntriess);
        }
        public static UserQuadTreeGetNEntriesResponse Failure(long ticket)
        {
            return new UserQuadTreeGetNEntriesResponse(ticket, false, null);
        }
    }
}
