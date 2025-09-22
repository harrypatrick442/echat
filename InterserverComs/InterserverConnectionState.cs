using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace InterserverComs
{
    [DataContract]
    public class InterserverConnectionState
    {
        private int _NodeId;
        [JsonPropertyName(InterserverConnectionStatusDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionStatusDataMemberNames.NodeId)]
        public int NodeId { get { return _NodeId; } protected set { _NodeId = value; } }
        private bool _GotResponsePingSuccessfully;
        [JsonPropertyName(InterserverConnectionStatusDataMemberNames.GotResponsePingSuccessfully)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionStatusDataMemberNames.GotResponsePingSuccessfully)]
        public bool GotResponsePingSuccessfully
        {
            get { return _GotResponsePingSuccessfully; }
            protected set { _GotResponsePingSuccessfully = value; }
        }
        private string _ErrorMessage;
        [JsonPropertyName(InterserverConnectionStatusDataMemberNames.Exception)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionStatusDataMemberNames.Exception)]
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            protected set { _ErrorMessage = value; }
        }
        [JsonPropertyName(InterserverConnectionStatusDataMemberNames.IsOpen)]
        [JsonInclude]
        [DataMember(Name = InterserverConnectionStatusDataMemberNames.IsOpen)]
        public bool IsOpen { get; }
        public InterserverConnectionState(int nodeId, bool isOpen, bool gotResponsePingSuccessfully, Exception exception) {
            _NodeId = nodeId;
            IsOpen = isOpen;
            _GotResponsePingSuccessfully = gotResponsePingSuccessfully;
            _ErrorMessage = exception?.ToString();
        }
        protected InterserverConnectionState() { }
    }
}
