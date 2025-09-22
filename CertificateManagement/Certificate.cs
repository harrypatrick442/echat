using Certes;
using Certes.Acme;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CertificateManagement
{
    [DataContract]
    public class Certificate
    {
        [JsonPropertyName("pem")]
        [JsonInclude]
        [DataMember(Name = "pem")]
        public PEM PEM{ get; protected set; }
        [JsonPropertyName("pfx")]
        [JsonInclude]
        [DataMember(Name = "pfx")]
        public PFX PFX { get; protected set; }
        [JsonPropertyName("createdAt")]
        [JsonInclude]
        [DataMember(Name = "createdAt")]
        public long CreatedAt { get; protected set; }
        [JsonPropertyName("expiresAt")]
        [JsonInclude]
        [DataMember(Name = "expiresAt")]
        public long? ExpiresAt { get; protected set; }
        [JsonPropertyName("previousCertificate")]
        [JsonInclude]
        [DataMember(Name = "previousCertificate")]
        public Certificate PreviousCertificate { get; protected set; }
        [JsonPropertyName("forDomains")]
        [JsonInclude]
        [DataMember(Name = "forDomains")]
        public string[] ForDomains { get; protected set; }
        public Certificate(PEM pem, PFX pfx, long createdAt, long? expiresAt, string[] forDomains, Certificate previousCertificate) {
            PEM = pem;
            PFX = pfx;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            PreviousCertificate = previousCertificate;
            ForDomains = forDomains;
        }
        protected Certificate() { }

    }
}
