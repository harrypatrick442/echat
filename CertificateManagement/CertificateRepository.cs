using Logging;
using JSON;
using DependencyManagement;
using CertificateManagement.Constants;

namespace CertificateManagement
{
    public static class CertificateRepository
    {
        private static readonly object _LockObject = new object();
        private static Certificate _Certificate;
        public static void Set(Certificate certificate, ILog log) {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            lock (_LockObject)
            {
                _Certificate = certificate;
                string json = Json.Serialize(certificate);
                string tlsCertificateJSONFilePath = DependencyManager.GetString(DependencyNames.TLSCertificateJSONFilePath);
                Directory.CreateDirectory(Path.GetDirectoryName(tlsCertificateJSONFilePath));
                File.WriteAllText(tlsCertificateJSONFilePath, json);
                log?.Info($"Wrote {nameof(Certificate)} to \"{tlsCertificateJSONFilePath}\"");

                Directory.CreateDirectory(TLS.CERTIFICATES_DIRECTORY_PATH);

                string fullChainFilePath = TLS.FULL_CHAIN_PATH;
                File.WriteAllText(fullChainFilePath, certificate.PEM.Cert);
                log?.Info($"Wrote \"{fullChainFilePath}\"");

                string privKeyFilePath = TLS.PRIV_KEY_PATH;
                File.WriteAllText(privKeyFilePath,
                    certificate.PEM.PrivateKey);
                log?.Info($"Wrote \"{privKeyFilePath}\"");
            }
        }
        public static Certificate Get()
        {
            lock (_LockObject)
            {
                if (_Certificate != null) return _Certificate;
                try
                {
                    string json = File.ReadAllText(DependencyManager.GetString(DependencyNames.TLSCertificateJSONFilePath));
                    if (string.IsNullOrEmpty(json))
                        return null;
                    _Certificate = Json.Deserialize<Certificate>(json);
                    return _Certificate;
                }
                catch(Exception ex) {
                    return null;
                }
            }
        }
    }
}