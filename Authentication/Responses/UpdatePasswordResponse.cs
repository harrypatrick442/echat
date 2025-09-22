using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Authentication.DataMemberNames.Responses;
using Core.Messages.Responses;

namespace Authentication.Responses
{
    [DataContract]
    public class UpdatePasswordResponse
    {
        [JsonPropertyName(UpdatePasswordResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = UpdatePasswordResponseDataMemberNames.Success,
            EmitDefaultValue = false)]

        public bool Success { get; set; }
        private UpdatePasswordResponse() { 
        
        }
        public UpdatePasswordResponse(bool success)
        {
            Success = success;
        }
    }
}
