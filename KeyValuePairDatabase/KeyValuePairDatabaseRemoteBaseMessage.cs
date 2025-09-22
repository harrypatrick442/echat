
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Ticketing;
using Core.Interfaces;
using Core.Messages.Messages;

namespace KeyValuePairDatabases
{
    [DataContract]
    public class KeyValuePairDatabaseRemoteBaseMessage:TicketedMessageBase, IInverseTicketed
    {
        private long _InverseTicket;
        [JsonPropertyName(InverseTicketedDataMemberNames.InverseTicket)]
        [JsonInclude]
        [DataMember(Name = InverseTicketedDataMemberNames.InverseTicket)]
        public long InverseTicket { get { return _InverseTicket; } set { _InverseTicket = value; } }
        public KeyValuePairDatabaseRemoteBaseMessage(string type) : base(type) {

        }
        protected KeyValuePairDatabaseRemoteBaseMessage() : base(null) { }
    }
}
