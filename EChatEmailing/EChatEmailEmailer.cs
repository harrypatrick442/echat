using Emailing;
using WebAPI.Emailing;

namespace EChatEmailing
{
    public class EChatEmailEmailer : IAuthenticationEmailer
    {
        public static readonly EChatEmailEmailer Instance = new EChatEmailEmailer();
        private static readonly SmtpServerConfiguration _SmtpServerConfiguration = new SmtpServerConfiguration(
            Configuration.SMTP_SERVER_USERNAME,
            Configuration.SMTP_SERVER_PASSWORD,
            Configuration.SMTP_SERVER_PORT,
            useSsl:true,
            Configuration.SMTP_SEVER_DOMAIN
            );
        public void SendPasswordResetEmail(string email, string username,
            string operatingSystem, string browserName,
            string actionUrl) {
            string message =
#if DEBUG
    File.ReadAllText("C:\\repos\\snippets\\API\\EChatEmailing\\EmailTemplates\\PasswordReset.html");
#else
    EmailTemplatesResource.PasswordReset;
#endif
            EmailHelper.Send(
                _SmtpServerConfiguration,
                fromEmailAddress: "awonderfulmachine9@gmail.com",
                toEmailAddress: email,
                subject: "Reset your E-Chat password",
                message,
                mapTagToCustomContent:new Dictionary<string, string> {
                    { "{name}", username },
                    { "{operating_system}", operatingSystem},
                    {"{browser_name}" , browserName},
                    { "{action_url}", actionUrl},
                    { "{support_url}", Configuration.SUPPORT_URL}
                }
            );
        }
    }
}
