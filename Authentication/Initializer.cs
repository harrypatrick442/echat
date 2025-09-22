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
        public static void Initialize(bool isDebug,
            IAuthenticationRelatedUserInfoSource authenticationRelatedUserInfoSource,
            CredentialsSetup credentialsSetup,
            IAuthenticationEmailer authenticationEmailing
        )
        {
            DalAuthenticationLocal.Initialize();
            int authenticationNodeId = isDebug
                ? GlobalConstants.Nodes.ECHAT_DEBUG 
                : GlobalConstants.Nodes.ECHAT_1;
            DalAuthentication dalAuthentication = 
                DalAuthentication.Initialize(authenticationNodeId);
            AuthenticationAttemptByIPFrequencyManager.Initialize();
            AuthenticationManager.Initialize(
                authenticationNodeId,
                DalAuthentication.Instance,
                authenticationRelatedUserInfoSource,
                credentialsSetup,
                authenticationEmailing
            );
            if (authenticationNodeId == Nodes.Nodes.Instance.MyId) {
                PasswordResetManager.Initialize(dalAuthentication);
            }
        }
    }
}