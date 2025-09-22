using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Requests
{
    [DataContract]
    [KnownType(typeof(UpdatePasswordRequest))]
    public class UpdatePasswordRequest
    {
        [JsonPropertyName(UpdatePasswordRequestDataMemberNames.Secret)]
        [JsonInclude]
        [DataMember(Name = UpdatePasswordRequestDataMemberNames.Secret)]
        public string Secret { get; protected set; }
        [JsonPropertyName(UpdatePasswordRequestDataMemberNames.Password)]
        [JsonInclude]
        [DataMember(Name = UpdatePasswordRequestDataMemberNames.Password)]
        public string Password { get; protected set; }

        protected UpdatePasswordRequest()
        {

        }
    }
}
