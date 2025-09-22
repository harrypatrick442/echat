using WebSocketSharp.Server;

namespace InterserverComs
{
    public static class Initializer
    {
        public static void Initialize(WebSocketServer webSocketServer)
        {
            InterserverPort.Initialize(webSocketServer, CertificateManagement.Constants.TLS.FULL_CHAIN_PATH);
            InterserverInverseTicketedSender.Initialize();
            InterserverTicketedSender.Initialize();
        }
    }
}