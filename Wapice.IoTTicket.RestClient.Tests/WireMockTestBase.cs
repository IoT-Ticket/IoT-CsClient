using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using WireMock.Matchers.Request;
using WireMock.Server;

namespace Wapice.IoTTicket.RestClient.Tests
{
    [TestClass]
    public abstract class WireMockTestBase
    {
        private const string PASSWORD = "pw";
        private const string USERNAME = "un";

        public IoTTicketClient Client{ get; private set; }
        public FluentMockServer Server { get; private set; }

        /// <summary>
        /// Verifies that <paramref name="request"/> was received and no other requests were received in test.
        /// </summary>
        /// <param name="request">Expected request.</param>
        public void AssertReceivedAsOnlyRequest(IRequestMatcher request)
        {
            var allReceivedRequests = Server.LogEntries;
            Assert.AreEqual(1, allReceivedRequests.Count());

            var matchedReceivedRequests = Server.FindLogEntries(request);
            Assert.AreEqual(1, matchedReceivedRequests.Count());
        }

        [TestInitialize]
        public void SetUpTest()
        {
            Server = CreateFluentMockServer();
            Client = CreateIoTTicketClient();
        }

        [TestCleanup]
        public void StopServer()
        {
            Server.Stop();
        }

        private IoTTicketClient CreateIoTTicketClient()
        {
            SecureString password = StringToSecureString(PASSWORD);
            return new IoTTicketClient(new Uri(Server.Urls.First()), USERNAME, password);
        }

        private FluentMockServer CreateFluentMockServer()
        {
            var fluentMockServer = FluentMockServer.Start();
            fluentMockServer.SetBasicAuthentication(USERNAME, PASSWORD);

            return fluentMockServer;
        }

        private SecureString StringToSecureString(string source)
        {
            SecureString secureStringPassword = new SecureString();
            source.ToList().ForEach(secureStringPassword.AppendChar);
            return secureStringPassword;
        }
    }
}
