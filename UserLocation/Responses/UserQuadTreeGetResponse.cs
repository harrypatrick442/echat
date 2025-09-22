using Core.DataMemberNames;
using Core.Messages.Messages;
using LocationCore;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserLocation.DataMemberNames.Responses;

namespace UserLocation
{
    [DataContract]
    public class UserQuadTreeGetResponse : TicketedMessageBase
    {
        [JsonPropertyName(UserQuadTreeGetResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name =UserQuadTreeGetResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(UserQuadTreeGetResponseDataMemberNames.Quadrants)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetResponseDataMemberNames.Quadrants)]
        public Quadrant[] Quadrants { get; protected set; }
        public UserQuadTreeGetResponse(long ticket, bool success, Quadrant[] quadrants)
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet)
        {
            Success = success;
            Quadrants = quadrants;
            Ticket = ticket;
        }
        protected UserQuadTreeGetResponse()
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet) { }
        public static UserQuadTreeGetResponse Successful(Quadrant[] quadrants, long ticket)
        {
            return new UserQuadTreeGetResponse(ticket, true, quadrants);
        }
        public static UserQuadTreeGetResponse Failure(long ticket)
        {
            return new UserQuadTreeGetResponse(ticket, false, null);
        }
    }
}
