using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using JSON;
using Core.Ticketing;
using Core.Messages.Messages;

namespace KeyValuePairDatabases
{
    [DataContract]
    public class RemoteOperationResponse: TypedInverseTicketedMessage
    {
        private bool _Success;
        [JsonPropertyName(RemoteOperationResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = RemoteOperationResponseDataMemberNames.Success, EmitDefaultValue =false)]
        public bool Success { get { return _Success; } protected set { _Success = value; } }
        private string _ErrorMessage;
        [JsonPropertyName(RemoteOperationResponseDataMemberNames.ErrorMessage)]
        [JsonInclude]
        [DataMember(Name = RemoteOperationResponseDataMemberNames.ErrorMessage, EmitDefaultValue = false)]
        public string ErrorMessage{ get { return _ErrorMessage; } protected set { _ErrorMessage = value; } }
        private string _Payload;
        [JsonPropertyName(RemoteOperationResponseDataMemberNames.Payload)]
        [JsonInclude]
        [DataMember(Name = RemoteOperationResponseDataMemberNames.Payload, EmitDefaultValue = false)]
        protected string Payload { get { return _Payload; } set { _Payload = value; } }
        public TPayload DeserializePayload<TPayload>()
        {
            return Json.Deserialize<TPayload>(_Payload);
        }
        public RemoteOperationResponse(
            long inverseTicket, string payload, bool success, string errorMessage)
            : base(InverseTicketedSender_MessageTypes.InverseTicketed)
        {
            _InverseTicket = inverseTicket;
            _Payload = payload;
            _Success = success;
            _ErrorMessage = errorMessage;
        }
        public RemoteOperationResponse(string payload, bool success, string errorMessage) : base(TicketedMessageType.Ticketed)
        {
            _Payload = payload;
            _Success = success;
            _ErrorMessage = errorMessage;
        }
        public RemoteOperationResponse(bool success, string errorMessage):base(TicketedMessageType.Ticketed)
        {
            _Success = success;
            _ErrorMessage = errorMessage;

        }
        public RemoteOperationResponse(Exception ex):base(TicketedMessageType.Ticketed)
        {
            _Success = false;
            _ErrorMessage = ex?.ToString();
        }
        protected RemoteOperationResponse() : base(TicketedMessageType.Ticketed) { }
        public static RemoteOperationResponse Successful() {
            return new RemoteOperationResponse(true, null);
        }
    }
}
