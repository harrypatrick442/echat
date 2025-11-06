using Microsoft.AspNetCore.Mvc;
using Logging;
using Authentication;
using Core.DTOs;
using Authentication.DAL;
using Microsoft.AspNetCore.Cors;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using JSON;
using Authentication.Requests;
using Authentication.Responses;

namespace WebAPI.Controllers
{
    [EnableCors("AuthenticationServerCors")]
    public class AuthenticationController : ControllerBase
    {
        private static readonly Regex _RegexExtractSecretFromResetPasswordLink = new Regex("\\/([0-9a-zA-Z]+)$");
        [HttpGet]
        [Route("resetPassword")]
        public void TestAuthentication() {

        }
        /// <summary>
        /// Called when they click the reset link
        /// </summary>
        /// <param name="email"></param>
        /// <param name="operatingSystem"></param>
        /// <param name="browserName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Configurations.Routes.RESET_PASSWORD_CLICKED_LINK)]
        public ContentResult ResetPasswordClickedLink()
        {
            try
            {
                Match match = _RegexExtractSecretFromResetPasswordLink.Match(Request.Path);
                if (match.Success)
                {
                    string secret = match.Groups[1].Value;
                    if (PasswordResetManager.Instance.GetUserForSecret(secret, out long userId))
                    {
                        return new ContentResult
                        {
                            ContentType = "text/html",
                            Content = ResetPasswordPages.UpdatePassword(
                                secret,
                                AuthenticationManager.Instance.CredentialsSetup.PasswordMinLength,
                                AuthenticationManager.Instance.CredentialsSetup.PasswordMaxLength
                            )
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            return new ContentResult
            {
                ContentType = "text/html",
                Content = ResetPasswordPages.LinkExpired()
            };
        }
        [HttpPost]
        [Route(Configurations.Routes.RESET_PASSWORD_UPDATE_PASSWORD)]
        public ContentResult ResetPasswordUpdatePassword()
        {
            UpdatePasswordResponse response = null;
            try
            {
                string body = new StreamReader(Request.Body).ReadToEnd();
                UpdatePasswordRequest request = Json.Deserialize<UpdatePasswordRequest>(body);
                if (PasswordResetManager.Instance.TryToUpdatePassword(request.Secret, request.Password))
                {
                    response = new UpdatePasswordResponse(true);
                }
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            if(response==null)
                response = new UpdatePasswordResponse(false);
            return new ContentResult
            {
                ContentType = "text/json",
                Content =Json.Serialize(response)
            };
        }
        [HttpGet]
        [Route(Configurations.Routes.RESET_PASSWORD_BY_EMAIL)]
        public void ResetPasswordByEmail()
        {
            try
            {
                throw new NotImplementedException();
                //PasswordResetManager.Prepare(authenticationInfo.UserId);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}