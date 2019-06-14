using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wapice.IoTTicket.RestClient.Exceptions;
using Wapice.IoTTicket.RestClient.Model;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Wapice.IoTTicket.RestClient.Tests
{
    [TestClass]
    public class IoTTicketClientDatanodeReadWriteTests : WireMockTestBase
    {

        private readonly Uri URI = new Uri("https://my.iot-ticket.com/api/v1/url");

        [TestMethod]
        public void TestGetDatanodesAsync_DeviceWithDatanodesExists_ReturnDatanodeDetailsSuccessfully()
        {
            const string DEVICE_ID = "id1";
            const int LIMIT = 1;
            const int OFFSET = 2;

            var datanodeDetail = new DatanodeDetail
            {
                DataType = LongDataType.TypeName,
                Name = "name",
                Unit = "unit",
                Url = URI
            };

            var datanodeDetails = new PagedResult<DatanodeDetail>
            {
                RequestedCount = LIMIT,
                Skip = OFFSET,
                TotalCount = 1,
                Result = new List<DatanodeDetail>
                {
                    datanodeDetail
                }
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/devices/" + DEVICE_ID + "/datanodes/")
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
                .WithBody(JsonSerializer.Serialize(datanodeDetails)));

            var result = Client.GetDatanodesAsync(DEVICE_ID, LIMIT, OFFSET).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public async Task TestGetDatanodesAsync_DeviceIdIsNotFound_ThrowIoTServerCommunicationException()
        {
            const string NOT_EXISTING_DEVICE_ID = "id2";
            const int LIMIT = 1;
            const int OFFSET = 2;

            var errorInfo = new ErrorInfo
            {
                Description = "Re-check device id and ensure device access is valid",
                Code = 8001,
                MoreInfoUrl = new Uri("https://my.iot-ticket.com/api/v1/errorcodes"),
                ApiVersion = 1
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/devices/" + NOT_EXISTING_DEVICE_ID + "/datanodes/")
                .WithHeader("Accept", "application/json")
                .WithParam("limit", LIMIT.ToString())
                .WithParam("offset", OFFSET.ToString())
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
                await Client.GetDatanodesAsync(NOT_EXISTING_DEVICE_ID, LIMIT, OFFSET);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException expectedException)
            {
                Assert.IsNotNull(expectedException.ErrorInfo);
                Assert.AreEqual(HttpStatusCode.Forbidden, expectedException.HttpStatusCode);
            }

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestWriteDatapointAsync_DeviceExists_WriteResultReturnedSuccessfully()
        {
            const string DEVICE_ID = "id1";

            var datanodeWritableValue = new DatanodeWritableValue
            {
                Name = "name",
                Path = "test/path",
                Value = 1,
                Unit = "unit"
            };

            var datanodeWritableValueCollection = new DatanodeWritableValue []
            {
                datanodeWritableValue
            };

            var datapointWriteResult = new DatapointWriteResult
            {
                Url = URI,
                WriteCount = 1
            };

            var writeResult = new WriteResult
            {
                DatapointWriteResults = new List<DatapointWriteResult>
                {
                    datapointWriteResult
                },
                TotalWriteCount = 1
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/process/write/" + DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher(JsonSerializer.Serialize(datanodeWritableValueCollection)))
                .UsingPost();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(writeResult)));
            
            var result = Client.WriteDatapointAsync(DEVICE_ID, datanodeWritableValue).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public async Task TestWriteDatapointAsync_DeviceIsNotFound_IoTServerCommunicationExceptionIsThrown()
        {
            const string NOT_EXISTING_DEVICE_ID = "id1";

            var datanodeWritableValue = new DatanodeWritableValue
            {
                Name = "name",
                Path = "test/path1",
                Value = 1,
                Unit = "unit"
            };

            var datanodeWritableValueCollection = new List<DatanodeWritableValue>
            {
                datanodeWritableValue,
            };

            var errorInfo = new ErrorInfo
            {
                Description = "Re-check device id and ensure device access is valid",
                Code = 8001,
                MoreInfoUrl = URI,
                ApiVersion = 1
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/process/write/" + NOT_EXISTING_DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher(JsonSerializer.Serialize(datanodeWritableValueCollection)))
                .UsingPost();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.Forbidden)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(errorInfo)));

            try
            {
                await Client.WriteDatapointCollectionAsync(NOT_EXISTING_DEVICE_ID, datanodeWritableValueCollection);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException expectedException)
            {
                Assert.IsNotNull(expectedException.ErrorInfo);
                Assert.AreEqual(HttpStatusCode.Forbidden, expectedException.HttpStatusCode);
            }

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestWriteDatapointCollectionAsync_DeviceExists_WriteResultReturnedSuccessfully()
        {
            const string DEVICE_ID = "id1";

            var datanodeWritableValue1 = new DatanodeWritableValue
            {
                Name = "name",
                Path = "test/path1",
                Value = 1,
                Unit = "unit"
            };

            var datanodeWritableValue2 = new DatanodeWritableValue
            {
                Name = "name",
                Path = "test/path2",
                Value = 1,
                Unit = "unit"
            };

            var datanodeWritableValueCollection = new List<DatanodeWritableValue>
            {
                datanodeWritableValue1,
                datanodeWritableValue2
            };

            var datapointWriteResult1 = new DatapointWriteResult
            {
                Url = URI,
                WriteCount = 1
            };

            var datapointWriteResult2 = new DatapointWriteResult
            {
                Url = URI,
                WriteCount = 1
            };

            var writeResult = new WriteResult
            {
                DatapointWriteResults = new List<DatapointWriteResult>
                {
                    datapointWriteResult1,
                    datapointWriteResult2
                },
                TotalWriteCount = 2
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/process/write/" + DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher(JsonSerializer.Serialize(datanodeWritableValueCollection)))
                .UsingPost();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(writeResult)));

            var result = Client.WriteDatapointCollectionAsync(DEVICE_ID, datanodeWritableValueCollection).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public async Task TestWriteDatapointCollectionAsync_DeviceIsNotFound_IoTServerCommunicationExceptionIsThrown()
        {
            const string NOT_EXISTING_DEVICE_ID = "id1";

            var datanodeWritableValue1 = new DatanodeWritableValue
            {
                Name = "name",
                Path = "test/path1",
                Value = 1,
                Unit = "unit"
            };

            var datanodeWritableValue2 = new DatanodeWritableValue
            {
                Name = "name",
                Path = "test/path2",
                Value = 1,
                Unit = "unit"
            };

            var datanodeWritableValueCollection = new List<DatanodeWritableValue>
            {
                datanodeWritableValue1,
                datanodeWritableValue2
            };

            var errorInfo = new ErrorInfo
            {
                Description = "Re-check device id and ensure device access is valid",
                Code = 8001,
                MoreInfoUrl = new Uri("https://my.iot-ticket.com/api/v1/errorcodes"),
                ApiVersion = 1
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/process/write/" + NOT_EXISTING_DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher(JsonSerializer.Serialize(datanodeWritableValueCollection)))
                .UsingPost();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.Forbidden)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(errorInfo)));

            try
            {
                await Client.WriteDatapointCollectionAsync(NOT_EXISTING_DEVICE_ID, datanodeWritableValueCollection);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException expectedException)
            {
                Assert.IsNotNull(expectedException.ErrorInfo);
                Assert.AreEqual(HttpStatusCode.Forbidden, expectedException.HttpStatusCode);
            }

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestReadProcessDataAsync_CriteriaWithRequiredPropertiesOnly_ProcessValuesReturnedSuccessfully()
        {
            const string DEVICE_ID = "id1";
            const string NAME = "name";
            const string UNIT = "unit";
            const string DATANODE = "test/path";

            DateTime startDate = new DateTime(2019, 1, 1);

            var datanodeQueryCriteria = new DatanodeQueryCriteria(DEVICE_ID, DATANODE);

            var datanodeReadValue = new DatanodeReadValue
            {
                Name = NAME,
                Path = DATANODE,
                Unit = UNIT,
                Values = new List<DatanodeValueData>
                {
                    new DatanodeValueData
                    {
                        StringValue = "1",
                        Timestamp = startDate
                    }
                }
            };

            var processValues = new ProcessValues
            {
                Url = URI,
                Datanodes = new List<DatanodeReadValue> {
                    datanodeReadValue
                }
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/process/read/" + DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithParam("datanodes", DATANODE)
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(processValues)));

            var result = Client.ReadProcessDataAsync(datanodeQueryCriteria).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestReadProcessDataAsync_CriteriaWithAllFields_ProcessValuesReturnedSuccessfully()
        {
            const string DEVICE_ID = "id1";
            const string NAME = "name";
            const string UNIT = "unit";
            const string DATANODE_1 = "test/path1";
            const string DATANODE_2 = "test/path2";
            const int LIMIT = 10;
            const string EXPECTED_QUERY = "?datanodes=test/path1,test/path2&order=descending&fromdate=1546293600000&todate=1546380000000&limit=10";

            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 1, 2);

            var datanodeQueryCriteria = new DatanodeQueryCriteria(DEVICE_ID, DATANODE_1, DATANODE_2)
            {
                Count = LIMIT,
                StartDate = startDate,
                EndDate = endDate,
                SortOrder = DatanodeQueryCriteria.Order.Descending
            };

            var datanodeReadValue = new DatanodeReadValue
            {
                Name = NAME,
                Path = DATANODE_1,
                Unit = UNIT,
                Values = new List<DatanodeValueData>
                {
                    new DatanodeValueData
                    {
                        StringValue = "1",
                        Timestamp = startDate
                    }
                }
            };

            var processValues = new ProcessValues
            {
                Url = URI,
                Datanodes = new List<DatanodeReadValue> {
                    datanodeReadValue
                }
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/process/read/" + DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(processValues)));
           
            var result = Client.ReadProcessDataAsync(datanodeQueryCriteria).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);

            // Wiremock.net does not support commas in url path variables, so have to check parameters manually.
            string receivedQuery = Server.LogEntries.First().RequestMessage.RawQuery;
            Assert.AreEqual(EXPECTED_QUERY, receivedQuery);
        }

        [TestMethod]
        public async Task TestReadProcessDataAsync_DeviceIdIsNotFound_ProcessValuesReturnedSuccessfully()
        {
            const string NOT_EXISTING_DEVICE_ID = "id2";
            const string DATANODE = "test/path";

            var datanodeQueryCriteria = new DatanodeQueryCriteria(NOT_EXISTING_DEVICE_ID, DATANODE);

            var errorInfo = new ErrorInfo
            {
                Description = "Re-check device id and ensure device access is valid",
                Code = 8001,
                MoreInfoUrl = new Uri("https://my.iot-ticket.com/api/v1/errorcodes"),
                ApiVersion = 1
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/process/read/" + NOT_EXISTING_DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
                .WithParam("datanodes", DATANODE)
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
                await Client.ReadProcessDataAsync(datanodeQueryCriteria);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException expectedException)
            {
                Assert.IsNotNull(expectedException.ErrorInfo);
                Assert.AreEqual(HttpStatusCode.Forbidden, expectedException.HttpStatusCode);
            }

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

    }
}
