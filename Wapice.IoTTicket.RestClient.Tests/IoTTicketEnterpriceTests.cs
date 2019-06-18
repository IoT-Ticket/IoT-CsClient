using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wapice.IoTTicket.RestClient.Model;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Wapice.IoTTicket.RestClient.Tests
{
    [TestClass]
    public class IoTTicketEnterpriceTests : WireMockTestBase
    {
        private readonly Uri URI = new Uri("https://my.iot-ticket.com/api/v1/url");

        [TestMethod]
        public async Task TestGetRootEnterprises_EnterprisesFound_ExpectedRequestReceived()
        {
            const int LIMIT = 10;
            const int OFFSET = 1;
            const int TOTAL_COUNT = 1;

            var enterprise = new Enterprise
            {
                Uri = URI,
                HasSubEnterprises = true,
                Name = "enterprise",
                ResourceId = "E100"
            };

            var enterprises = new PagedResult<Enterprise>
            {
                RequestedCount = LIMIT,
                Skip = OFFSET,
                TotalCount = TOTAL_COUNT,
                Result = new List<Enterprise>
                {
                    enterprise
                }
            };

            var expectedRequest =
               Request
               .Create()
               .WithPath("/enterprises/")
               .WithHeader("Accept", "application/json")
               .WithParam("limit", LIMIT.ToString())
               .WithParam("offset", OFFSET.ToString())
               .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(enterprises)));

            await Client.GetRootEnterpricesAsync(LIMIT, OFFSET);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestGetRootEnterprises_EnterprisesFound_EnterpriseDeserializedSuccessfully()
        {
            const int LIMIT = 10;
            const int OFFSET = 1;
            const int TOTAL_COUNT = 1;

            var enterprise = new Enterprise
            {
                Uri = URI,
                HasSubEnterprises = true,
                Name = "enterprise",
                ResourceId = "E100"
            };

            var enterprises = new PagedResult<Enterprise>
            {
                RequestedCount = LIMIT,
                Skip = OFFSET,
                TotalCount = TOTAL_COUNT,
                Result = new List<Enterprise>
                {
                    enterprise
                }
            };

            var expectedRequest =
               Request
               .Create()
               .WithPath("/enterprises/")
               .WithHeader("Accept", "application/json")
               .WithParam("limit", LIMIT.ToString())
               .WithParam("offset", OFFSET.ToString())
               .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(enterprises)));

            var result = Client.GetRootEnterpricesAsync(LIMIT, OFFSET).Result;

            Assert.AreEqual(enterprises.RequestedCount, result?.RequestedCount);
            Assert.AreEqual(enterprises.TotalCount, result?.TotalCount);
            Assert.AreEqual(enterprises.Skip, result?.Skip);
            Assert.AreEqual(enterprises.Result.Count(), result?.Result?.Count());

            var actualEnterprise = result?.Result?.First();

            Assert.AreEqual(enterprise.Name, actualEnterprise?.Name);
            Assert.AreEqual(enterprise.ResourceId, actualEnterprise?.ResourceId);
            Assert.AreEqual(enterprise.HasSubEnterprises, actualEnterprise?.HasSubEnterprises);
            Assert.AreEqual(enterprise.Uri, actualEnterprise?.Uri);
        }

        [TestMethod]
        public async Task TestGetSubEnterprises_EnterprisesFound_ExpectedRequestReceived()
        {
            const string ENTERPRISE_ID = "100";

            const int LIMIT = 10;
            const int OFFSET = 1;
            const int TOTAL_COUNT = 1;

            var enterprise = new Enterprise
            {
                Uri = URI,
                HasSubEnterprises = true,
                Name = "enterprise",
                ResourceId = "E100"
            };

            var enterprises = new PagedResult<Enterprise>
            {
                RequestedCount = LIMIT,
                Skip = OFFSET,
                TotalCount = TOTAL_COUNT,
                Result = new List<Enterprise>
                {
                    enterprise
                }
            };

            var expectedRequest =
               Request
               .Create()
               .WithPath("/enterprises/" + ENTERPRISE_ID + "/")
               .WithHeader("Accept", "application/json")
               .WithParam("limit", LIMIT.ToString())
               .WithParam("offset", OFFSET.ToString())
               .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(enterprises)));

            await Client.GetSubEnterpricesAsync(ENTERPRISE_ID, LIMIT, OFFSET);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestGetSubEnterprises_EnterprisesFound_EnterpriseDeserializedSuccessfully()
        {
            const string ENTERPRISE_ID = "1000";

            const int LIMIT = 10;
            const int OFFSET = 1;
            const int TOTAL_COUNT = 1;

            var enterprise = new Enterprise
            {
                Uri = URI,
                HasSubEnterprises = true,
                Name = "enterprise",
                ResourceId = "E100"
            };

            var enterprises = new PagedResult<Enterprise>
            {
                RequestedCount = LIMIT,
                Skip = OFFSET,
                TotalCount = TOTAL_COUNT,
                Result = new List<Enterprise>
                {
                    enterprise
                }
            };

            var expectedRequest =
               Request
               .Create()
               .WithPath("/enterprises/" + ENTERPRISE_ID + "/")
               .WithHeader("Accept", "application/json")
               .WithParam("limit", LIMIT.ToString())
               .WithParam("offset", OFFSET.ToString())
               .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(enterprises)));

            var result = Client.GetSubEnterpricesAsync(ENTERPRISE_ID, LIMIT, OFFSET).Result;

            Assert.AreEqual(enterprises.RequestedCount, result?.RequestedCount);
            Assert.AreEqual(enterprises.TotalCount, result?.TotalCount);
            Assert.AreEqual(enterprises.Skip, result?.Skip);
            Assert.AreEqual(enterprises.Result.Count(), result?.Result?.Count());

            var actualEnterprise = result?.Result?.First();

            Assert.AreEqual(enterprise.Name, actualEnterprise?.Name);
            Assert.AreEqual(enterprise.ResourceId, actualEnterprise?.ResourceId);
            Assert.AreEqual(enterprise.HasSubEnterprises, actualEnterprise?.HasSubEnterprises);
            Assert.AreEqual(enterprise.Uri, actualEnterprise?.Uri);

        }

    }
}
