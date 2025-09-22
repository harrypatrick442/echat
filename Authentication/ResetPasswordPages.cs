
using Core.Exceptions;
using Authentication.DAL;
using System.Timers;
using Timer = System.Timers.Timer;
using Core.Collections;
using Core.Strings;
using GlobalConstants;

namespace Authentication
{
    public class ResetPasswordPages
    {
        public static string UpdatePassword(string secret, int passwordMinLength, int passwordMaxLength)
        {
            string html =
#if DEBUG
            File.ReadAllText("C:\\repos\\snippets\\API\\Authentication\\Pages\\UpdatePassword.html");
#else
            Pages.Pages.UpdatePassword;
#endif
#if DEBUG
            string origin = "https://localhost:7161";
#else
                    string domainName = GlobalConstants.Nodes.FirstUniqueDomainForNode(
                        GlobalConstants.Nodes.AuthenticationNodeId);
                    string origin = $"https://{domainName}";
#endif
            string url= $"{origin}/{RoutesWithoutSlash.RESET_PASSWORD_UPDATE_PASSWORD}";

            return StringHelper.Format(html, new Dictionary<string, string>
            {
                {"{url}", url},
                {"{secret}", secret},
                { "{passwordMinLength}", passwordMinLength.ToString()},
                { "{passwordMaxLength}", passwordMaxLength.ToString()}
            });
        }
        public static string LinkExpired()
        {
            string html =
#if DEBUG
            File.ReadAllText("C:\\repos\\snippets\\API\\Authentication\\Pages\\LinkExpired.html");
#else
            Pages.Pages.LinkExpired;
#endif
            return html;
        }
    }
}
