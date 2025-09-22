using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logging;
using Shutdown;
using WebSocketSharp.Server;
using UserRouting;

namespace Testing
{
    [TestClass]
    public class UserRoutingTableUnitTests
    {

        [TestInitialize]
        public void Initialize()
        {
            ShutdownManager.Initialize((code) => { }, ()=>Logs.Default, throwErrorOnAlreadyInitialized:false);
            WebSocketServer webSocketServer = new WebSocketServer();
            //_UserRoutingTable = new UserRoutingTable(nodes, webSocketServer);
        }

        [TestMethod]
        public void Test()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
            //_TicketedSender.Dispose();
        }
    }
}
