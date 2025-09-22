using Certes;
using Certes.Acme;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CertificateManagement
{
    [DataContract]
    public class PFX
    {
        [JsonPropertyName("friendlyName")]
        [JsonInclude]
        [DataMember(Name = "friendlyName")]
        public string FriendlyName { get; protected set; }
        [JsonPropertyName("password")]
        [JsonInclude]
        [DataMember(Name = "password")]
        public string Password { get; protected set; }
        [JsonPropertyName("bytes")]
        [JsonInclude]
        [DataMember(Name = "bytes")]
        public byte[] Bytes { get; protected set; }
        public PFX(string friendlyName, string password, byte[] bytes) {
            FriendlyName = friendlyName;
            Password = password;
            Bytes = bytes;
        }
        protected PFX() { }
    }
}
