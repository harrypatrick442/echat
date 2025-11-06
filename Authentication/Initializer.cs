using Core.Exceptions;
using JSON;
using Shutdown;
using WebSocketSharp.Server;
using Nodes;
using MessageTypes.Internal;
using Core.Messages.Messages;
using Logging;
using Core;
using Authentication.DAL;
using Emailing;

namespace Authentication
{
    public static class Initializer
    {
        public static void Initialize(
            IAuthenticationRelatedUserInfoSource authenticationRelatedUserInfoSource,
            CredentialsSetup credentialsSetup,
            IAuthenticationEmailer authenticationEmailing,
            int nodeId
        )
        {
            DalAuthenticationLocal.Initialize();
            DalAuthentication dalAuthentication = 
                DalAuthentication.Initialize(nodeId);
            AuthenticationAttemptByIPFrequencyManager.Initialize();
            AuthenticationManager.Initialize(
                nodeId,
                DalAuthentication.Instance,
                authenticationRelatedUserInfoSource,
                credentialsSetup,
                authenticationEmailing
            );
            if (nodeId == Nodes.Nodes.Instance.MyId) {
                PasswordResetManager.Initialize(dalAuthentication);
            }
        }
    }
}