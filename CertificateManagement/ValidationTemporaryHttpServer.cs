using Certes.Acme;
using Logging;
using Snippets;
namespace CertificateManagement
{
    public sealed class ValidationTemporaryHttpServer:IDisposable
    {
        private SimpleHTTPServer _SimpleHTTPServer;
        public ValidationTemporaryHttpServer(IEnumerable<IChallengeContext>challenges)
        {
            string rootDirectoryPathOfServer = $"/var/snippets_acmechallenge/";
            string fullDirectoryPath = rootDirectoryPathOfServer + ".well-known/acme-challenge/";
            Directory.CreateDirectory(fullDirectoryPath);
            foreach (var challenge in challenges)
            {
                Logs.Default.Info($"keyAuthz: {challenge.KeyAuthz}");
                Logs.Default.Info($"token: {challenge.Token}");
                string pathToWriteChallengeTo = fullDirectoryPath + challenge.Token;
                File.WriteAllText(pathToWriteChallengeTo, challenge.KeyAuthz);
            }
            _SimpleHTTPServer = new SimpleHTTPServer(rootDirectoryPathOfServer, port: 80, allowCors: true,
                handleException: (ex) => Logs.Default.Error(ex));

        }
        public void Dispose() {
            _SimpleHTTPServer?.Dispose();
        }
    }
}