using Authentication.DAL;
using Authentication.Requests;
using Authentication.Responses;
using ControllerHelpers;
using Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs;
using Core.Enums;
using Core.Exceptions;
using JSON;
using Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseMessages.DataMemberNames;

namespace Authentication
{
    public class ControllerSimpleAuthenticationManager
    {
        private readonly SingleUserHardCodedDalAuthentication _DalAuthentication;
        private readonly SimpleSessionIdSource _SimpleSessionIdSource = new SimpleSessionIdSource();
        private SessionInfo _SessionInfo;
        private readonly object _LockObject = new object();
        private bool _Disposed = false;
        public ControllerSimpleAuthenticationManager(string hash) {
            _DalAuthentication = new SingleUserHardCodedDalAuthentication(
                new AuthenticationInfo(1, hash, null, /*null,*/ null),
                anyCredentialsReturnAuthenticationInfo: true);
        }
        private void Authenticate(HttpRequest httpRequest)
        {
#if DEBUG 
            return;
#endif
            string token = httpRequest.Cookies[TokenDataMemberName.Token];
            if (string.IsNullOrEmpty(token))
                throw new NotAuthenticatedException("Not signed in");
            lock (_LockObject)
            {
                if (_SessionInfo == null || _SessionInfo.Token != token)
                    throw new NotAuthenticatedException("Not signed in");
                return;
            }
        }
        public ActionResult AuthenticateWithToken(HttpRequest request,
            HttpResponse response)
        {

            return ControllerHelper.ToJson<AuthenticateResponse>(request, () =>
            {
                try
                {
                    string token = request.Cookies[TokenDataMemberName.Token];
                    if (string.IsNullOrEmpty(token))
                        return AuthenticateResponse.Failed(0, AuthenticationFailedReason.BadCredentials, null, 0);
                    lock (_LockObject)
                    {
                        if (_SessionInfo == null)
                            return AuthenticateResponse.Failed(0, AuthenticationFailedReason.BadCredentials, null, 0);
                    }
                    SessionInfo sessionInfo = AuthenticationManager.Instance.TryToAuthenticateWithToken(
                        token, null, _SimpleSessionIdSource);
                    lock (_LockObject)
                    {
                        if (_Disposed)
                        {
                            sessionInfo?.Dispose();
                            throw new ObjectDisposedException(nameof(ControllerSimpleAuthenticationManager));
                        }
                        _SessionInfo = sessionInfo;
                    }
                    response.Cookies.Append(TokenDataMemberName.Token, sessionInfo.Token,
                        new CookieOptions
                        {
                            HttpOnly = false,
                            IsEssential = true,
                            Secure = false,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTime.UtcNow.AddDays(3)
                        });
                    return AuthenticateResponse.Successful(0, 0, sessionInfo.Token, sessionInfo.SessionId, null);
                }
                catch (MustWaitToRetryException mEx)
                {

                    return AuthenticateResponse.Failed(0,
                        AuthenticationFailedReason.TooManyAttempts, mEx.SecondssDelay, 0);
                }
                catch (BadCredentialsException)
                {
                    return AuthenticateResponse.Failed(0,
                        AuthenticationFailedReason.BadCredentials, null, 0);
                }
                catch (BusyException)
                {
                    return AuthenticateResponse.Failed(0,
                        AuthenticationFailedReason.Busy, null, 0);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                    return AuthenticateResponse.Failed(0,
                        AuthenticationFailedReason.Unknown, null, 0);
                }

            });
        }
        public ActionResult SignIn(HttpRequest httpRequest, 
            HttpResponse httpResponse, HttpContext httpContext)
        {
            return ControllerHelper.JsonToJson<LogInRequest, AuthenticateResponse>(httpRequest, (request) => {
                try
                {
                    lock (_LockObject)
                    {
                        _SessionInfo?.Dispose();
                        _SessionInfo = null;
                        if (_Disposed) throw new ObjectDisposedException(nameof(ControllerSimpleAuthenticationManager));
                    }
                    SessionInfo sessionInfo = AuthenticationManager.Instance.LogIn(request, httpContext.Connection.RemoteIpAddress,
                        null, _SimpleSessionIdSource);
                    lock (_LockObject)
                    {
                        if (_Disposed)
                        {
                            sessionInfo?.Dispose();
                            throw new ObjectDisposedException(nameof(ControllerSimpleAuthenticationManager));
                        }
                        _SessionInfo = sessionInfo;
                    }
                    httpResponse.Cookies.Append(TokenDataMemberName.Token, sessionInfo.Token,
                        new CookieOptions
                        {
                            HttpOnly = false,
                            IsEssential = true,
                            Secure = false,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTime.UtcNow.AddDays(3)
                        });
                    return AuthenticateResponse.Successful(request.Ticket, 0, sessionInfo.Token, sessionInfo.SessionId, null);
                }
                catch (MustWaitToRetryException mEx)
                {

                    return AuthenticateResponse.Failed(request.Ticket,
                        AuthenticationFailedReason.TooManyAttempts, mEx.SecondssDelay, 0);
                }
                catch (BadCredentialsException)
                {
                    return AuthenticateResponse.Failed(request.Ticket,
                        AuthenticationFailedReason.BadCredentials, null, 0);
                }
                catch (BusyException)
                {
                    return AuthenticateResponse.Failed(request.Ticket,
                        AuthenticationFailedReason.Busy, null, 0);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                    return AuthenticateResponse.Failed(request.Ticket,
                        AuthenticationFailedReason.Unknown, null, 0);
                }
            });
        }
        public ActionResult AuthenticatedJsonToJson<TRequest, TResponse>(HttpRequest httpRequest,
            Func<TRequest, TResponse> callback)
        {
            try
            {
                Authenticate(httpRequest);
                return ControllerHelper.JsonToJson(httpRequest, callback);
            }
            catch (AuthenticationException)
            {
                return new StatusCodeResult(401);
            }
        }
        ~ControllerSimpleAuthenticationManager() {
            Dispose();
        }
        public void Dispose() {
            lock(_LockObject)
            {
                if (_Disposed) return;
                _Disposed = true;
                _SessionInfo?.Dispose();
            }
        }
    }
}
