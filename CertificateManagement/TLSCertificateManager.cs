using Certes;
using Certes.Acme;
using Logging;
using Core.Exceptions;
using Core.Maths;
using Core.Timing;
using Certes.Acme.Resource;
using Certes.Pkcs;
using Shutdown;
using Core;
using DependencyManagement;
namespace CertificateManagement
{
    public static class TLSCertificateManager
    {
        private const int TIMEOUT_VALIDATE_AFTER_MILLISECONDS = 240000;
        private const long RENEW_MILLISECONDS_AFTER_CREATE = 2L * 30 * 24 * 60 * 60 * 1000;
        #region Public
        public static Certificate DoYourThing(int nodeId, bool verbose = true,
            CancellationToken? cancellationToken = null)
        {
            return DoYourThing(GlobalConstants.Nodes.GetDomainsForNode(nodeId), verbose, cancellationToken);
        }
        public static Certificate DoYourThing(string[] myDomains, bool verbose = true, 
            CancellationToken? cancellationToken = null)
        {
            ILog log = verbose ? Logs.Default : null;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            List<CancellationTokenRegistration> cancellationTokenRegistrations = new List<CancellationTokenRegistration>();
            cancellationTokenRegistrations.Add(ShutdownManager.Instance.CancellationToken
                .Register(cancellationTokenSource.Cancel));
            if (cancellationToken != null) {
                cancellationTokenRegistrations.Add(((CancellationToken)cancellationToken)
                    .Register(cancellationTokenSource.Cancel));
            }
            try
            {
                return DoYourThing_CancellationTakenCareOf(myDomains, log, cancellationTokenSource.Token);
            }
            finally {
                foreach (CancellationTokenRegistration registration in cancellationTokenRegistrations)
                {
                    registration.Unregister();
                }
            }
        }
        #endregion Public
        #region Private
        private static Certificate DoYourThing_CancellationTakenCareOf(string[] myDomains, ILog log, CancellationToken cancellationToken)
        {
            Certificate certificate = CertificateRepository.Get();
            try
            {
                if (certificate != null)
                {
                    log?.Info("Read the certificate object from disk successfully!");
                    bool certificateContainsAllDomains = CertificateContainsAllDomains(certificate, myDomains, out string[] missingDomains);
                    if (!certificateContainsAllDomains)
                    {
                        log?.Info($"The domains {string.Join(',', missingDomains.Select(m => $"\"{m}\""))} were missing");
                    }
                    else
                    {

                        log?.Info($"No domains were missing");
                    }
                    if ((!CertificateExpiringAndNeedsRenewing(certificate, log))
                        && certificateContainsAllDomains)
                        return certificate;
                    log?.Info("Certificate needs renewing!");
                    if (CheckForDontTryAgain(log))
                    {
                        log?.Info($"YOU MUST CLEAR \"{DependencyManager.GetString(DependencyNames.DontTryCertifyAgainFilePath)}\" OR CERTIFICATE WONT RENEW");
                        return certificate;
                    }
                }
                else
                {
                    if (CheckForDontTryAgain(log))
                        return certificate;
                }
                cancellationToken.ThrowIfCancellationRequested();
                Firewall.Initialize().OpenPortsUntilShutdown(80);
                log?.Info("Opened port 80 on the firewall");
                Task<Certificate> taskGenerateNewCertificate = GenerateNewCertificate(myDomains, log, cancellationToken);
                taskGenerateNewCertificate.Wait(cancellationToken);
                return taskGenerateNewCertificate.Result;
            }
            catch (Exception ex)
            {
                WriteDontTryCertifyAgain();
                log?.Error(new OperationFailedException($"Generation or certificate failed", ex));
                return certificate;
            }
        }
        private static bool CertificateContainsAllDomains(Certificate certificate, string[] domains,
            out string[] missingDomains) {
            if (certificate.ForDomains == null) {
                missingDomains = domains;
                return domains.Length <= 0;
            }

            List<string> missingDomainsList = new List<string>();
            foreach (string domain in domains) {
                if (!certificate.ForDomains.Contains(domain))
                    missingDomainsList.Add(domain);
            }
            missingDomains = missingDomainsList.ToArray();
            return missingDomainsList.Count() < 1;
        }
        private static bool CertificateExpiringAndNeedsRenewing(Certificate certificate, ILog log) {
            //if (certificate.ExpiresAt == null) return false;
            //log?.Info("Certificate had expiration date!");
            log?.Info($"Created at {certificate.CreatedAt} and expires at {certificate.ExpiresAt}");
            return certificate.CreatedAt+ RENEW_MILLISECONDS_AFTER_CREATE < TimeHelper.MillisecondsNow;
        }
        private static bool CheckForDontTryAgain(ILog log)
        {
            string dontTryCertificateAgainFilePath = DependencyManager.GetString(DependencyNames.DontTryCertifyAgainFilePath);
            bool dontTryAgain = File.Exists(dontTryCertificateAgainFilePath);
            if (dontTryAgain)
                log?.Info($"Not trying again because found \"{dontTryCertificateAgainFilePath}\"");
            return dontTryAgain;
        }
        private static Certificate CreateCertificate(CertificateChain cert, IKey privateKey,
            Order orderResource, string[] forDomains) {

            PEM pem = CreatePEM(cert, privateKey);
            PFX pfx = CreatePFX(cert, privateKey);
            long now = TimeHelper.MillisecondsNow;
            long? expiresAt = GetExpiresAt(orderResource);
            return new Certificate(pem, pfx, now, expiresAt, forDomains, CertificateRepository.Get());
        }
        private static string CreateNewAccount(string email)
        {

            var acme = new AcmeContext(WellKnownServers.LetsEncryptV2);
            var taskAccount = acme.NewAccount(email, true);
            taskAccount.Wait();
            var account = taskAccount.Result;
            var pemKey = acme.AccountKey.ToPem();
            return pemKey;
        }
        private static PEM CreatePEM(CertificateChain certificateChain, IKey privateKey)
        {
            return new PEM(certificateChain.ToPem(), privateKey.ToPem());
        }
        private static PFX CreatePFX(CertificateChain cert, IKey privateKey)
        {

            var pfxBuilder = cert.ToPfx(privateKey);
            string friendlyName = "my-cert";
            string password = RandomHelper.RandomString(16);
            var pfxBytes = pfxBuilder.Build(friendlyName, password);
            return new PFX(friendlyName, password, pfxBytes);

        }
        private static CertificationRequestBuilder GenerateCertificateRequest(string[] myDomains)
        {
            var certificateRequestBuilder = new CertificationRequestBuilder();
            certificateRequestBuilder.AddName("c", "GB");
            certificateRequestBuilder.AddName("st", "East Sussex");
            certificateRequestBuilder.AddName("l", "Newhaven");
            certificateRequestBuilder.AddName("o", "OneStoneSolutions");
            certificateRequestBuilder.AddName("cn", myDomains.First());
            foreach (string domain in myDomains.Skip(1))
                certificateRequestBuilder.SubjectAlternativeNames.Add(domain);
            return certificateRequestBuilder;
        }
        private static async Task<Certificate> GenerateNewCertificate(string[] myDomains, ILog log, CancellationToken cancellationToken)
        {

            AcmeContext acme = await SignIn();
            log?.Info("Signed in successfully");
            cancellationToken.ThrowIfCancellationRequested();   
            Certificate certificate = await Order(acme, myDomains, log, cancellationToken);
            return certificate;
        }
        private static long? GetExpiresAt(Order orderResource)
        {
            DateTimeOffset? dateTimeOffset = orderResource.Expires;
            return dateTimeOffset.HasValue? dateTimeOffset.Value.ToUnixTimeMilliseconds() : null;
        }
        private static async Task<Certificate> Order(AcmeContext acme, string[] domains, ILog log, CancellationToken cancellationToken)
        {
                IOrderContext order = await acme.NewOrder(domains);
                cancellationToken.ThrowIfCancellationRequested();
                await Validate(order, log, cancellationToken);
                log?.Info("Validated all successfully!");
                CertificationRequestBuilder csrBuilder =
                    GenerateCertificateRequest(domains);
                byte[] csr = csrBuilder.Generate();
                IKey privateKey = csrBuilder.Key;
                await order.Finalize(csr);
                log?.Info("Finalized successfully!");
                var orderResource = await order.Resource();
                var cert = await order.Download();
                log?.Info("Downloaded successfully!");
                Certificate certificate = CreateCertificate(cert, privateKey, orderResource, domains);
                CertificateRepository.Set(certificate, log);
                log?.Info($"Set certificate in {nameof(CertificateRepository)} successfully!");
                return certificate;
        }
        private static async Task<AcmeContext> SignIn()
        {
            var accountKey = KeyFactory.FromPem(Keys.AccountPemKey);
            var acme = new AcmeContext(WellKnownServers.LetsEncryptV2, accountKey);
            await acme.Account();
            return acme;
        }
        private static async Task Validate(IOrderContext order, ILog log, CancellationToken cancellationToken)
        {
            var authorizations = await order.Authorizations();
            List<IChallengeContext> challenges = new List<IChallengeContext>();
            foreach (var auth in authorizations)
            {
                var challenge = await auth.Http();
                cancellationToken.ThrowIfCancellationRequested();
                challenges.Add(challenge);
            }
            List<Exception> exceptions = new List<Exception>();
            using (ValidationTemporaryHttpServer server = new ValidationTemporaryHttpServer(challenges))
            {
                CountdownLatch countdownLatch = new CountdownLatch(challenges.Count);
                foreach (var challenge in challenges)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            Validate(challenge, log, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                        }
                        finally
                        {
                            countdownLatch.Signal();
                        }
                    }).Start();
                }
                countdownLatch.Wait(cancellationToken);
                if (exceptions.Any())
                    throw new AggregateException(exceptions);
            }
        }
        private static void Validate(IChallengeContext httpChallenge, ILog log, CancellationToken cancellationToken)
        {
            long startedWaingAt = TimeHelper.MillisecondsNow;
            Task<Challenge> taskChallenge= httpChallenge.Validate();
            taskChallenge.Wait(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            Challenge challenge = taskChallenge.Result;
            while (true)
            {
                Thread.Sleep(3000);
                cancellationToken.ThrowIfCancellationRequested();
                ChallengeStatus? challengeStatus = challenge.Status;
                if (challengeStatus == null)
                    throw new OperationFailedException($"{nameof(ChallengeStatus)} was null");
                if (challengeStatus.Equals(ChallengeStatus.Valid))
                {
                    log?.Info("Valid!");
                    return;
                }
                if (challengeStatus.Equals(ChallengeStatus.Invalid))
                    throw new OperationFailedException("Invalid challenge status");
                if (challengeStatus.Equals(ChallengeStatus.Pending)
                    || challengeStatus.Equals(ChallengeStatus.Processing))
                {
                    log?.Info("Pending...");
                }
                long timePassed = TimeHelper.MillisecondsNow - startedWaingAt;
                if (timePassed > TIMEOUT_VALIDATE_AFTER_MILLISECONDS)
                    throw new OperationFailedException("Gave up waiting");
            }
        }
        private static void WriteDontTryCertifyAgain()
        {
            try
            {
                string dontTryCertificateAgainFilePath = DependencyManager.GetString(DependencyNames.DontTryCertifyAgainFilePath);
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(dontTryCertificateAgainFilePath));
                File.WriteAllText(dontTryCertificateAgainFilePath, TimeHelper.MillisecondsNow.ToString());
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
            }
        }
        #endregion Private
    }
}