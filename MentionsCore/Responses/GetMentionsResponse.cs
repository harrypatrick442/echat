using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MentionsCore.Messages;
using MentionsCore.DataMemberNames.Responses;

namespace MentionsCore.Responses
{
    [DataContract]
    public class GetMentionsResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetMentionsResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetMentionsResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(GetMentionsResponseDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = GetMentionsResponseDataMemberNames.Entries)]
        public Mention[]? Entries{ get; protected set; }
        protected GetMentionsResponse(bool successful, Mention[]? mentions, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Entries = mentions;
            _Ticket = ticket;
        }
        protected GetMentionsResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetMentionsResponse Success(Mention[] mentions,
             long ticket)
        {
            return new GetMentionsResponse(true, mentions, ticket);
        }
        public static GetMentionsResponse Failed(long ticket)
        {
            return new GetMentionsResponse(false, null, ticket);
        }
    }
}
