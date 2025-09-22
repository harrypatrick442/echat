
using DevOne.Security.Cryptography.BCrypt;
using Core.Exceptions;
using Logging;
using WebSocketSharp;
using Nodes;
using Snippets.WebAPI.WebsocketServers;

namespace InterserverComs
{
    public abstract class AuthenticatedNodeWebsocketServerBase : WebsocketServerBase
    {
        public const string PASSWORD_QUERY_STRING_KEY = "password";
        public const string NODE_QUERY_STRING_KEY = "node";
        protected INodes _Nodes;
        protected INode _NodeThisEndpointGoesTo;
        private bool _AuthenticationDebuggingEnabled;
        protected AuthenticatedNodeWebsocketServerBase(bool authenticationDebuggingEnabled = false)
        {
            _AuthenticationDebuggingEnabled = authenticationDebuggingEnabled;
        }


        protected override void OnOpen()
        {
            base.OnOpen();
            try
            {
                string password = Context?.QueryString[PASSWORD_QUERY_STRING_KEY];
                if (string.IsNullOrEmpty(password))
                    throw new BadCredentialsException("password parameter was invalid");
                string node = Context?.QueryString[NODE_QUERY_STRING_KEY];
                if (string.IsNullOrEmpty(node))
                    throw new BadCredentialsException("node parameter was invalid");
                int otherNodeId = int.Parse(node);
                INode nodeMe = _Nodes.Me;
                INode otherNode = _Nodes.GetNodeById(otherNodeId);
                if (otherNode == null)
                    throw new BadCredentialsException($"node with id {otherNodeId} did not exist");
                InterserverConnection interserverConnectionOnMeToOtherNode = nodeMe.GetInterserverConnectionTo(otherNodeId);
                if(interserverConnectionOnMeToOtherNode==null)
                    throw new BadCredentialsException($"No interserver connection to {otherNodeId} existed on node {nodeMe.Id}");
                string clientIpAddress = _ClientIPAddress.ToString();
                string expectedIpAddress = interserverConnectionOnMeToOtherNode.ExpectedIPAddressOfClient;
                if (expectedIpAddress != null && expectedIpAddress != clientIpAddress)
                    throw new RejectedClientException($"The ip address of the client {clientIpAddress} did not match the expected ip address {expectedIpAddress}");
                if (!BCryptHelper.CheckPassword(password, interserverConnectionOnMeToOtherNode.Hash))
                    throw new BadCredentialsException("password was incorrect");
                _NodeThisEndpointGoesTo = otherNode;
            }
            catch(Exception ex) {
                Dispose();
                if (_AuthenticationDebuggingEnabled)
                {
                    Logs.Default.Error(ex);
                }
                throw new Exception("Authenticating incoming node connection failed", ex);
            }
        }
        public bool IsOpen { get { return base.State.Equals(WebSocketState.Open); } }
        protected virtual void Initialize(
            INodes nodes)
        {
            _Nodes = nodes;
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}