using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JSON;
using Core.Messages.Messages;
using KeyValuePairDatabase;

namespace KeyValuePairDatabases
{
    [DataContract]
    public class RemoteOperationRequest : TicketedMessageBase
    {
        private int _DatabaseIdentifier;
        [JsonPropertyName(RemoteOperationRequestDataMemberNames.DatabaseIdentifier)]
        [JsonInclude]
        [DataMember(Name =RemoteOperationRequestDataMemberNames.DatabaseIdentifier)]
        public int DatabaseIdentifier { get { return _DatabaseIdentifier; } protected set { _DatabaseIdentifier = value; } }
        private object _Identifier;
        [JsonPropertyName(RemoteOperationRequestDataMemberNames.Identifier)]
        [JsonInclude]
        [DataMember(Name = RemoteOperationRequestDataMemberNames.Identifier)]
        public object Identifier { get { return _Identifier; } protected set { _Identifier = value; } }
        private Operation _Operation;
        [JsonPropertyName(RemoteOperationRequestDataMemberNames.Operation)]
        [JsonInclude]
        [DataMember(Name = RemoteOperationRequestDataMemberNames.Operation)]
        public Operation Operation { get { return _Operation; } protected set { _Operation = value; } }
        private string _Payload;
        [JsonPropertyName(RemoteOperationRequestDataMemberNames.Payload)]
        [JsonInclude]
        [DataMember(Name = RemoteOperationRequestDataMemberNames.Payload)]
        public string Payload { get { return _Payload; } protected set { _Payload = value; } }
        public TPayload DeserializePayload<TPayload>()
        {
            return Json.Deserialize<TPayload>(_Payload);
        }

        public RemoteOperationRequest(int databaseIdentifier, string payload,
            Operation operation, object identifier) : base(InterserverMessageTypes.KeyValuePairDatabaseRequest)
        {
            _Payload = payload;
            _DatabaseIdentifier = databaseIdentifier;
            _Operation = operation;
            _Identifier = identifier;
        }

        public RemoteOperationRequest(int databaseIdentifier, Operation operation,
            object identifier) : base(InterserverMessageTypes.KeyValuePairDatabaseRequest)
        {
            _DatabaseIdentifier = databaseIdentifier;
            _Operation = operation;
            _Identifier = identifier;
        }
        protected RemoteOperationRequest() : base(InterserverMessageTypes.KeyValuePairDatabaseRequest) { }
    }
}
