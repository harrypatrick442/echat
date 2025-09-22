using Certes;
using Certes.Acme;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CertificateManagement
{
    [DataContract]
    public class PEM
    {
        [JsonPropertyName("cert")]
        [JsonInclude]
        [DataMember(Name = "cert")]
        public string Cert { get; protected set; }
        [JsonPropertyName("privateKey")]
        [JsonInclude]
        [DataMember(Name = "privateKey")]
        public string PrivateKey { get; protected set; }
        public PEM(string cert, string privateKey) {
            Cert = cert;
            PrivateKey = privateKey;
        }
        protected PEM() { }
    }
}
