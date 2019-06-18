using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wapice.IoTTicket.RestClient.Exceptions;
using Wapice.IoTTicket.RestClient.Extensions;
using Wapice.IoTTicket.RestClient.Model;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Wapice.IoTTicket.RestClient.Tests
{
    [TestClass]
    public class IoTTicketClientStatisticsTests : WireMockTestBase
    {
        private readonly Uri URI = new Uri("https://my.iot-ticket.com/api/v1/url");

        [TestMethod]
        public async Task TestReadStatisticalDataAsync_WithRequiredCriteriaOnly_ExpectedRequestReceived()
        {
            const string DEVICE_ID = "deviceId";
            DateTime FROM_DATE = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime TO_DATE = new DateTime(2019, 2, 1, 0, 0, 0);
            const string DATAPOINT = "test/path";
            const StatisticalDataQueryCriteria.Grouping GROUPING = StatisticalDataQueryCriteria.Grouping.Day;

            var criteria = new StatisticalDataQueryCriteria(DEVICE_ID, GROUPING, FROM_DATE, TO_DATE, DATAPOINT);

            var statisticalValueData = new StatisticalValueData
            {
                Average = 1.0,
                Count = 1,
                Maximum = 1.0,
                Minimum = 1.0,
                Sum = 1.0,
                Timestamp = FROM_DATE
            };

            var statisticalDatanodeReadValue = new StatisticalDatanodeReadValue
            {
                DataType = LongDataType.TypeName,
                Name = "name",
                Unit = "unit",
                Path = DATAPOINT,
                Values = new List<StatisticalValueData>
                {
                    statisticalValueData
                }
            };

            var statisticalValues = new StatisticalValues
            {
                Url = URI,
                Datanodes = new List<StatisticalDatanodeReadValue>
                {
                    statisticalDatanodeReadValue
                }
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/stat/read/" + DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithParam("fromdate", FROM_DATE.ToUnixTime().ToString())
                .WithParam("todate", TO_DATE.ToUnixTime().ToString())
                .WithParam("grouping", GROUPING.ToString().ToLower())
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(statisticalValues)));

            await Client.ReadStatisticalDataAsync(criteria);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public async Task TestReadStatisticalDataAsync_WithAllCriteria_ExpectedRequestReceived()
        {
            const string DEVICE_ID = "deviceId";
            DateTime FROM_DATE = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime TO_DATE = new DateTime(2019, 2, 1, 0, 0, 0);
            const string DATAPOINT_1 = "test/path1";
            const string DATAPOINT_2 = "test/path2";
            const StatisticalDataQueryCriteria.Grouping GROUPING = StatisticalDataQueryCriteria.Grouping.Day;
            const string EXPECTED_QUERY = "?datanodes=test/path1,test/path2&fromdate=1546293600000&todate=1548972000000&grouping=day&order=descending&vtags=vtag1,vtag2";

            var criteria = new StatisticalDataQueryCriteria(DEVICE_ID, GROUPING, FROM_DATE, TO_DATE, DATAPOINT_1, DATAPOINT_2)
            {
                SortOrder = DatanodeQueryCriteria.Order.Descending,
                VTags = new List<string>
                {
                    "vtag1",
                    "vtag2"
                }
            };

            var statisticalValueData = new StatisticalValueData
            {
                Average = 1.0,
                Count = 1,
                Maximum = 1.0,
                Minimum = 1.0,
                Sum = 1.0,
                Timestamp = FROM_DATE
            };

            var statisticalDatanodeReadValue = new StatisticalDatanodeReadValue
            {
                DataType = LongDataType.TypeName,
                Name = "name",
                Unit = "unit",
                Path = DATAPOINT_1,
                Values = new List<StatisticalValueData>
                {
                    statisticalValueData
                }
            };

            var statisticalValues = new StatisticalValues
            {
                Url = URI,
                Datanodes = new List<StatisticalDatanodeReadValue>
                {
                    statisticalDatanodeReadValue
                }
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/stat/read/" + DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithParam("fromdate", FROM_DATE.ToUnixTime().ToString())
                .WithParam("todate", TO_DATE.ToUnixTime().ToString())
                .WithParam("grouping", GROUPING.ToString().ToLower())
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(statisticalValues)));

            await Client.ReadStatisticalDataAsync(criteria);

            AssertReceivedAsOnlyRequest(expectedRequest);

            // Wiremock.net does not support commas in url path variables, so have to check parameters manually.
            string receivedQuery = Server.LogEntries.First().RequestMessage.RawQuery;
            Assert.AreEqual(EXPECTED_QUERY, receivedQuery);
        }

        [TestMethod]
        public void TestReadStatisticalDataAsync_DeviceIdIsFound_StatisticalValuesDeserializedSuccessfully()
        {
            const string DEVICE_ID = "deviceId";
            DateTime FROM_DATE = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime TO_DATE = new DateTime(2019, 2, 1, 0, 0, 0);
            const string DATAPOINT = "test/path";
            const StatisticalDataQueryCriteria.Grouping GROUPING = StatisticalDataQueryCriteria.Grouping.Day;

            var criteria = new StatisticalDataQueryCriteria(DEVICE_ID, GROUPING, FROM_DATE, TO_DATE, DATAPOINT);

            var statisticalValueData = new StatisticalValueData
            {
                Average = 1.0,
                Count = 1,
                Maximum = 1.0,
                Minimum = 1.0,
                Sum = 1.0,
                Timestamp = FROM_DATE
            };

            var statisticalDatanodeReadValue = new StatisticalDatanodeReadValue
            {
                DataType = LongDataType.TypeName,
                Name = "name",
                Unit = "unit",
                Path = DATAPOINT,
                Values = new List<StatisticalValueData>
                {
                    statisticalValueData
                }
            };

            var statisticalValues = new StatisticalValues
            {
                Url = URI,
                Datanodes = new List<StatisticalDatanodeReadValue>
                {
                    statisticalDatanodeReadValue
                }
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/stat/read/" + DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithParam("fromdate", FROM_DATE.ToUnixTime().ToString())
                .WithParam("todate", TO_DATE.ToUnixTime().ToString())
                .WithParam("grouping", GROUPING.ToString().ToLower())
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(statisticalValues)));

            var result = Client.ReadStatisticalDataAsync(criteria).Result;

            Assert.AreEqual(statisticalValues.Url, result?.Url);
            Assert.AreEqual(statisticalValues.Datanodes.Count(), result?.Datanodes?.Count());

            var actualStatisticalDatanodeReadValue = result?.Datanodes?.First();

            Assert.AreEqual(statisticalDatanodeReadValue.Name, actualStatisticalDatanodeReadValue?.Name);
            Assert.AreEqual(statisticalDatanodeReadValue.Path, actualStatisticalDatanodeReadValue?.Path);
            Assert.AreEqual(statisticalDatanodeReadValue.Unit, actualStatisticalDatanodeReadValue?.Unit);
            Assert.AreEqual(statisticalDatanodeReadValue.DataType, actualStatisticalDatanodeReadValue?.DataType);
            Assert.AreEqual(statisticalDatanodeReadValue.Values.Count(), actualStatisticalDatanodeReadValue?.Values?.Count());

            var actualStatisticalValueData = actualStatisticalDatanodeReadValue?.Values.First();

            Assert.AreEqual(statisticalValueData.Count, actualStatisticalValueData?.Count);
            Assert.AreEqual(statisticalValueData.Maximum, actualStatisticalValueData?.Maximum);
            Assert.AreEqual(statisticalValueData.Average, actualStatisticalValueData?.Average);
            Assert.AreEqual(statisticalValueData.Minimum, actualStatisticalValueData?.Minimum);
            Assert.AreEqual(statisticalValueData.Sum, actualStatisticalValueData?.Sum);
            Assert.AreEqual(statisticalValueData.Timestamp, actualStatisticalValueData?.Timestamp);
        }

        [TestMethod]
        public async Task TestReadStatisticalDataAsync_DeviceIdIsNotFound_IoTServerCommunicationExceptionWithForbiddenStatusIsThrowns()
        {
            const string NOT_EXISTING_DEVICE_ID = "deviceId2";
            DateTime FROM_DATE = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime TO_DATE = new DateTime(2019, 2, 1, 0, 0, 0);
            const string DATAPOINT = "test/path";
            const StatisticalDataQueryCriteria.Grouping GROUPING = StatisticalDataQueryCriteria.Grouping.Day;

            var criteria = new StatisticalDataQueryCriteria(NOT_EXISTING_DEVICE_ID, GROUPING, FROM_DATE, TO_DATE, DATAPOINT);

            var errorInfo = new ErrorInfo
            {
                Description = "Re-check device id and ensure device access is valid",
                Code = 8001,
                MoreInfoUrl = new Uri("https://my.iot-ticket.com/api/v1/errorcodes"),
                ApiVersion = 1,
            };


            var expectedRequest =
                Request
                .Create()
                .WithPath("/stat/read/" + NOT_EXISTING_DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithParam("fromdate", FROM_DATE.ToUnixTime().ToString())
                .WithParam("todate", TO_DATE.ToUnixTime().ToString())
                .WithParam("grouping", GROUPING.ToString().ToLower())
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.Forbidden)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(errorInfo)));

            try
            {
                await Client.ReadStatisticalDataAsync(criteria);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(HttpStatusCode.Forbidden, actualException.HttpStatusCode);
            }

        }

        [TestMethod]
        public async Task TestReadStatisticalDataAsync_DeviceIdIsNotFound_ErrorInfoIsDeserializedSuccessfully()
        {
            const string NOT_EXISTING_DEVICE_ID = "deviceId2";
            DateTime FROM_DATE = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime TO_DATE = new DateTime(2019, 2, 1, 0, 0, 0);
            const string DATAPOINT = "test/path";
            const StatisticalDataQueryCriteria.Grouping GROUPING = StatisticalDataQueryCriteria.Grouping.Day;

            var criteria = new StatisticalDataQueryCriteria(NOT_EXISTING_DEVICE_ID, GROUPING, FROM_DATE, TO_DATE, DATAPOINT);

            var errorInfo = new ErrorInfo
            {
                Description = "Re-check device id and ensure device access is valid",
                Code = 8001,
                MoreInfoUrl = new Uri("https://my.iot-ticket.com/api/v1/errorcodes"),
                ApiVersion = 1,
            };


            var expectedRequest =
                Request
                .Create()
                .WithPath("/stat/read/" + NOT_EXISTING_DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithParam("fromdate", FROM_DATE.ToUnixTime().ToString())
                .WithParam("todate", TO_DATE.ToUnixTime().ToString())
                .WithParam("grouping", GROUPING.ToString().ToLower())
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.Forbidden)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(errorInfo)));

            try
            {
                await Client.ReadStatisticalDataAsync(criteria);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(errorInfo.ApiVersion, actualException?.ErrorInfo?.ApiVersion);
                Assert.AreEqual(errorInfo.Code, actualException?.ErrorInfo?.Code);
                Assert.AreEqual(errorInfo.Description, actualException?.ErrorInfo?.Description);
                Assert.AreEqual(errorInfo.MoreInfoUrl, actualException?.ErrorInfo?.MoreInfoUrl);
            }

        }

    }
}
