using Core.Messages.Messages;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KeyValuePairDatabases.Appended
{
    [DataContract]
    public class AppendedAppendResponse : TicketedMessageBase
    {
        private bool _Successful;
        [JsonPropertyName(AppendedAppendResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name =AppendedAppendResponseDataMemberNames.Successful)]
        public bool Successful { get { return _Successful; }protected set { _Successful = value;  } }
        [JsonPropertyName(AppendedAppendResponseDataMemberNames.IndexToContinueFromToGoBackFromMessage)]
        [JsonInclude]
        [DataMember(Name =AppendedAppendResponseDataMemberNames.IndexToContinueFromToGoBackFromMessage, EmitDefaultValue =false)]
        public long? IndexToContinueFromToGoBackFromMessage { get; protected set; }

        public AppendedAppendResponse(bool successful, long? indexToContinueFromToGoBackFromMessage, long ticket)
            :base(TicketedMessageType.Ticketed)
        {
            _Successful = successful;
            IndexToContinueFromToGoBackFromMessage = indexToContinueFromToGoBackFromMessage;
            _Ticket = ticket;
        }
        protected AppendedAppendResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}
