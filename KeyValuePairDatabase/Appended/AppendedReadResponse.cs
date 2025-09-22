using Core.Messages.Messages;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KeyValuePairDatabases.Appended
{
    [DataContract]
    public class AppendedReadResponse : TicketedMessageBase
    {
        private bool _Successful;
        [JsonPropertyName(AppendedReadResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = AppendedReadResponseDataMemberNames.Successful)]
        public bool Successful { get { return _Successful; } protected set { _Successful = value;  } }

        private string[] _Entries;
        [JsonPropertyName(AppendedReadResponseDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = AppendedReadResponseDataMemberNames.Entries)]
        public string[] Entries
        {
            get { return _Entries; }
            protected set { _Entries = value; }
        }
        private long _ToIndexFromBeginningExclusive;
        [JsonPropertyName(AppendedReadResponseDataMemberNames.ToIndexFromBeginningExclusive)]
        [JsonInclude]
        [DataMember(Name = AppendedReadResponseDataMemberNames.ToIndexFromBeginningExclusive)]
        public long ToIndexFromBeginningExclusive
        {
            get { return _ToIndexFromBeginningExclusive; }
            protected set { _ToIndexFromBeginningExclusive = value; }
        }
        protected AppendedReadResponse(bool successful, string[] entries, 
            long toIndexFromBeginningExclusive, long ticket) : base(TicketedMessageType.Ticketed)
        {
            _Entries = entries;
            _ToIndexFromBeginningExclusive = toIndexFromBeginningExclusive;
            _Ticket = ticket;
        }
        protected AppendedReadResponse() : base(TicketedMessageType.Ticketed) { }
        public static AppendedReadResponse Success(string[] entries, long toIndexFromBeginningExclusive, long ticket)
        {
            return new AppendedReadResponse(true, entries, toIndexFromBeginningExclusive, ticket);
        }
        public static AppendedReadResponse Failed(long ticket)
        {
            return new AppendedReadResponse(false, null, 0, ticket);
        }
    }
}
