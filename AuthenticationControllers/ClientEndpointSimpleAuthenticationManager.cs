using Authentication.DAL;
using Authentication.Requests;
using Authentication.Responses;
using Logging;
using Core.DTOs;
using Core.Enums;
using Core.Exceptions;
using JSON;
using Core.Messages;
using System.Net;
using Sessions;

namespace Authentication
{
    public class ClientEndpointSimpleAuthenticationManager
    {
        private readonly SingleUserHardCodedDalAuthentication _DalAuthentication;
        private readonly SimpleSessionIdSource _SimpleSessionIdSource = new SimpleSessionIdSource();
        private readonly object _SessionInfoLock = new object();
        private SessionInfo _SessionInfo;
        private Action<string> _SendJSONString;
        public ClientEndpointSimpleAuthenticationManager(string hash, Action<string> sendJSONString) {
            _DalAuthentication = new SingleUserHardCodedDalAuthentication(
                new AuthenticationInfo(1, hash, null, /*null,*/ null),
                anyCredentialsReturnAuthenticationInfo: true);
            _SendJSONString = sendJSONString;
        }
        public void LogIn(TypeTicketedAndWholePayload e, IPAddress clientIPAddress)
        {
            _RequestHandleResponse<LogInRequest, AuthenticateResponse>(e, (request) =>
            {
                try
                {
                    lock (_SessionInfoLock)
                    {
                        _SessionInfo = null;
                    }
                    SessionInfo sessionInfo = AuthenticationManager.Instance.LogIn(request, clientIPAddress,
                        null, _SimpleSessionIdSource);
                    lock (_SessionInfoLock)
                    {
                        _SessionInfo = sessionInfo;
                    }
                    return AuthenticateResponse.Successful(
                        request.Ticket, 0, sessionInfo.Token, sessionInfo.SessionId, null);
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
        public bool Authenticated
        {
            get
            {
                lock (_SessionInfoLock)
                    return _SessionInfo!=null;
            }
        }
        private void _RequestHandleResponse<TRequest, TResponse>(TypeTicketedAndWholePayload e,
            Func<TRequest, TResponse> callback)
        {
            TRequest request = Json.Deserialize<TRequest>(e.JsonString);
            TResponse response = callback(request);
            _SendJSONString(Json.Serialize(response));
        }
    }
}
