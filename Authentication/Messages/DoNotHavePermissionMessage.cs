using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Authentication.DataMemberNames.Messages;
using Core.Messages.Messages;
namespace Authentication.Messages
{
    [DataContract]
    public class DoNotHavePermissionMessage : TypedMessageBase
    {
        private string _Message;
        [JsonPropertyName(DoNotHavePermissionMessageDataMemberNames.Message)]
        [JsonInclude]
        [DataMember(Name = DoNotHavePermissionMessageDataMemberNames.Message)]
        public string Message { get { return _Message; } protected set { _Message = value; } }
        public DoNotHavePermissionMessage(string message)
        {
            _Type = global::MessageTypes.MessageTypes.AuthenticationDoNotHavePermission;
            _Message = message;
        }
        protected DoNotHavePermissionMessage() { 
            
        }
    }
}
