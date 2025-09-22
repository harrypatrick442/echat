using Certes;
using Certes.Acme;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CertificateManagement
{
    public static class DependencyNames
    {
        public const string 
            TLSCertificateJSONFilePath = "TLSCertificateJSONFilePath",
            DontTryCertifyAgainFilePath= "DontTryCertifyAgainFilePath";
    }
}
