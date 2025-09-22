using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KeyValuePairDatabases.Appended
{
    [DataContract]
    public class AppendedReadRequest: TicketedMessageBase
    {
        private int _DatabaseIdentifier;
        [JsonPropertyName(AppendedReadRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = AppendedReadRequestDataMemberNames.DatabaseIdentifier)]
        public int DatabaseIdentifier { get { return _DatabaseIdentifier; } protected set { _DatabaseIdentifier = value; } }
        private long _Identifier;
        [JsonPropertyName(AppendedReadRequestDataMemberNames.Identifier)]
        [JsonInclude]
        [DataMember(Name = AppendedReadRequestDataMemberNames.Identifier)]
        public long Identifier { get { return _Identifier; } protected set { _Identifier = value; } }
        private int _NEntries;
        [JsonPropertyName(AppendedReadRequestDataMemberNames.NEntries)]
        [JsonInclude]
        [DataMember(Name = AppendedReadRequestDataMemberNames.NEntries)]
        public int NEntries { get { return _NEntries; } protected set { _NEntries = value; } }
        private long? _IndexToReadFromBackwardsExclusive;
        [JsonPropertyName(AppendedReadRequestDataMemberNames.IndexToReadFromBackwardsExclusive)]
        [JsonInclude]
        [DataMember(Name = AppendedReadRequestDataMemberNames.IndexToReadFromBackwardsExclusive)]
        public long? IndexToReadFromBackwardsExclusive { 
            get { return _IndexToReadFromBackwardsExclusive; }
            protected set { _IndexToReadFromBackwardsExclusive = value; }
        }

        public AppendedReadRequest(long identifier, int nEntries, long? indexToReadFromBackwardsExclusive, int databaseIdentifier)
            : base(InterserverMessageTypes.AppendedRead)
        {
            _Identifier = identifier;
            _NEntries = nEntries;
            _IndexToReadFromBackwardsExclusive = indexToReadFromBackwardsExclusive;
            _DatabaseIdentifier = databaseIdentifier;
        }
        protected AppendedReadRequest()
            : base(InterserverMessageTypes.AppendedRead) { }
    }
}
