using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class Example
    {
        [JsonPropertyName("username")]
        [JsonInclude]
        [DataMember(Name = "username")]
        public string Username { get; protected set; }
        [JsonPropertyName("password")]
        [JsonInclude]
        [DataMember(Name = "password")]
        public string Password { get; protected set; }
    }
}
