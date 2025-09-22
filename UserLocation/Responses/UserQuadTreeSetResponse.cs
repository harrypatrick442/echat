using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserLocation.DataMemberNames.Responses;

namespace UserLocation
{
    [DataContract]
    public class UserQuadTreeSetResponse : TicketedMessageBase
    {
        [JsonPropertyName(UserQuadTreeSetResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeSetResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public UserQuadTreeSetResponse(long ticket, bool success)
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet)
        {
            Success = success;
            Ticket = ticket;
        }
        protected UserQuadTreeSetResponse()
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet) { }
        public static UserQuadTreeSetResponse Successful(long ticket)
        {
            return new UserQuadTreeSetResponse(ticket, true);
        }
        public static UserQuadTreeSetResponse Failure(long ticket)
        {
            return new UserQuadTreeSetResponse(ticket, false);
        }
    }
}
