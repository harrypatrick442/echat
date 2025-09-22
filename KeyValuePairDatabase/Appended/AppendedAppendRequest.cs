using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KeyValuePairDatabases.Appended
{
    [DataContract]
    public class AppendedAppendRequest : TicketedMessageBase
    {
        private int _DatabaseIdentifier;
        [JsonPropertyName(AppendedAppendRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name = AppendedAppendRequestDataMemberNames.DatabaseIdentifier)]
        public int DatabaseIdentifier { get { return _DatabaseIdentifier; } protected set { _DatabaseIdentifier = value; } }
        private long _Identifier;
        [JsonPropertyName(AppendedAppendRequestDataMemberNames.Identifier)]
        [JsonInclude]
        [DataMember(Name = AppendedAppendRequestDataMemberNames.Identifier)]
        public long Identifier { get { return _Identifier; } protected set { _Identifier = value; } }
        private string _Entry;
        [JsonPropertyName(AppendedAppendRequestDataMemberNames.Entry)]
        [JsonInclude]
        [DataMember(Name = AppendedAppendRequestDataMemberNames.Entry)]
        public string Entry { get { return _Entry; } protected set { _Entry = value; } }
        public AppendedAppendRequest(long identifier, string entry, int databaseIdentifier)
            : base(InterserverMessageTypes.AppendedAppend)
        {
            _Identifier = identifier;
            _Entry = entry;
            _DatabaseIdentifier = databaseIdentifier;
        }
        protected AppendedAppendRequest()
            : base(InterserverMessageTypes.AppendedAppend) { }

    }
}
