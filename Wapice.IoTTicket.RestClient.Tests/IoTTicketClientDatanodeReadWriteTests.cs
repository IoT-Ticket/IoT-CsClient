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
        public async Task TestGetDatanodesAsync_DeviceWithDatanodesExists_ExpectedRequestIsReceived()
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

            await Client.GetDatanodesAsync(DEVICE_ID, LIMIT, OFFSET);
           
            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestGetDatanodesAsync_DeviceWithDatanodesExists_DatanodeDetailsDeserializedSuccessfully()
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
            Assert.AreEqual(datanodeDetails.RequestedCount, result?.RequestedCount);
            Assert.AreEqual(datanodeDetails.Skip, result?.Skip);
            Assert.AreEqual(datanodeDetails.TotalCount, result?.TotalCount);
            Assert.AreEqual(datanodeDetails.Result.Count(), result?.Result?.Count());

            var actualDatanodeDetail = datanodeDetails?.Result?.First();

            Assert.AreEqual(datanodeDetail.DataType, actualDatanodeDetail?.DataType);
            Assert.AreEqual(datanodeDetail.Name, actualDatanodeDetail?.Name);
            Assert.AreEqual(datanodeDetail.Unit, actualDatanodeDetail?.Unit);
            Assert.AreEqual(datanodeDetail.Url, actualDatanodeDetail?.Url);
        }

        [TestMethod]
        public async Task TestGetDatanodesAsync_DeviceIdIsNotFound_ThrowIoTServerCommunicationExceptionWithForbiddenStatus()
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
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(HttpStatusCode.Forbidden, actualException.HttpStatusCode);
            }
        }

        [TestMethod]
        public async Task TestGetDatanodesAsync_DeviceIdIsNotFound_ErrorInfoDeserializedSuccessfully()
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
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(errorInfo.ApiVersion, actualException?.ErrorInfo?.ApiVersion);
                Assert.AreEqual(errorInfo.Code, actualException?.ErrorInfo?.Code);
                Assert.AreEqual(errorInfo.Description, actualException?.ErrorInfo?.Description);
                Assert.AreEqual(errorInfo.MoreInfoUrl, actualException?.ErrorInfo?.MoreInfoUrl);
            }
        }

        [TestMethod]
        public void TestWriteDatapointAsync_DeviceExists_ExpectedRequestIsReceived()
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

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestWriteDatapointAsync_DeviceExists_WriteResultIsDeserializedSuccessfully()
        {
            const string DEVICE_ID = "id1";

            var datanodeWritableValue = new DatanodeWritableValue
            {
                Name = "name",
                Path = "test/path",
                Value = 1,
                Unit = "unit"
            };

            var datanodeWritableValueCollection = new DatanodeWritableValue[]
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

            Assert.AreEqual(writeResult.TotalWriteCount, result?.TotalWriteCount);
            Assert.AreEqual(writeResult.DatapointWriteResults.Count(), result?.DatapointWriteResults?.Count());

            var actualDatapointWriteResult = result?.DatapointWriteResults?.First();
            Assert.AreEqual(datapointWriteResult.WriteCount, actualDatapointWriteResult?.WriteCount);
            Assert.AreEqual(datapointWriteResult.Url, actualDatapointWriteResult?.Url);
        }

        [TestMethod]
        public async Task TestWriteDatapointAsync_DeviceIsNotFound_IoTServerCommunicationExceptionIsThrownWithForbiddenStatus()
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
                Assert.AreEqual(HttpStatusCode.Forbidden, expectedException.HttpStatusCode);
            }
        }

        [TestMethod]
        public async Task TestWriteDatapointAsync_DeviceIsNotFound_ErrorInfoIsDeserializedCorrectly()
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
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(errorInfo.ApiVersion, actualException?.ErrorInfo?.ApiVersion);
                Assert.AreEqual(errorInfo.Code, actualException?.ErrorInfo?.Code);
                Assert.AreEqual(errorInfo.Description, actualException?.ErrorInfo?.Description);
                Assert.AreEqual(errorInfo.MoreInfoUrl, actualException?.ErrorInfo?.MoreInfoUrl);
            }
        }

        [TestMethod]
        public async Task TestWriteDatapointCollectionAsync_DeviceExists_ExpectedRequestIsReceived()
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

            await Client.WriteDatapointCollectionAsync(DEVICE_ID, datanodeWritableValueCollection);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }


        [TestMethod]
        public void TestWriteDatapointCollectionAsync_DeviceExists_WriteResultDeserializedSuccessfully()
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
            Assert.AreEqual(writeResult.TotalWriteCount, result?.TotalWriteCount);
            Assert.AreEqual(writeResult.DatapointWriteResults.Count(), result?.DatapointWriteResults?.Count());

            var actualDatapointWriteResult1 = result?.DatapointWriteResults?.ElementAt(0);
            Assert.AreEqual(datapointWriteResult1.WriteCount, actualDatapointWriteResult1?.WriteCount);
            Assert.AreEqual(datapointWriteResult1.Url, actualDatapointWriteResult1?.Url);

            var actualDatapointWriteResult2 = result?.DatapointWriteResults?.ElementAt(0);
            Assert.AreEqual(datapointWriteResult2.WriteCount, actualDatapointWriteResult2?.WriteCount);
            Assert.AreEqual(datapointWriteResult2.Url, actualDatapointWriteResult2?.Url);
        }

        [TestMethod]
        public async Task TestWriteDatapointCollectionAsync_DeviceIsNotFound_IoTServerCommunicationExceptionIsThrownWithForbiddenStatus()
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
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(HttpStatusCode.Forbidden, actualException.HttpStatusCode);
            }
        }

        [TestMethod]
        public async Task TestWriteDatapointCollectionAsync_DeviceIsNotFound_ErrorInfoIsDeserializedCorrectly()
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
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(errorInfo.ApiVersion, actualException?.ErrorInfo?.ApiVersion);
                Assert.AreEqual(errorInfo.Code, actualException?.ErrorInfo?.Code);
                Assert.AreEqual(errorInfo.Description, actualException?.ErrorInfo?.Description);
                Assert.AreEqual(errorInfo.MoreInfoUrl, actualException?.ErrorInfo?.MoreInfoUrl);
            }
        }

        [TestMethod]
        public async Task TestReadProcessDataAsync_CriteriaWithRequiredPropertiesOnly_ExpectedRequestReceived()
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

            await Client.ReadProcessDataAsync(datanodeQueryCriteria);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public async Task TestReadProcessDataAsync_CriteriaWithAllFields_ExpectedRequestReceived()
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
           
            await Client.ReadProcessDataAsync(datanodeQueryCriteria);

            AssertReceivedAsOnlyRequest(expectedRequest);

            // Wiremock.net does not support commas in url path variables, so have to check parameters manually.
            string receivedQuery = Server.LogEntries.First().RequestMessage.RawQuery;
            Assert.AreEqual(EXPECTED_QUERY, receivedQuery);
        }

        [TestMethod]
        public void TestReadProcessDataAsync_DeviceIdIsFound_ProcessValuesDeserializedCorrectly()
        {
            const string DEVICE_ID = "id1";
            const string NAME = "name";
            const string UNIT = "unit";
            const string DATANODE = "test/path";

            DateTime startDate = new DateTime(2019, 1, 1);

            var datanodeQueryCriteria = new DatanodeQueryCriteria(DEVICE_ID, DATANODE);

            var datanodeValueData = new DatanodeValueData
            {
                StringValue = "1",
                Timestamp = startDate
            };

            var datanodeReadValue = new DatanodeReadValue
            {
                Name = NAME,
                Path = DATANODE,
                Unit = UNIT,
                Values = new List<DatanodeValueData>
                {
                    datanodeValueData
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

            Assert.AreEqual(processValues.Url, result?.Url);
            Assert.AreEqual(processValues.Datanodes.Count(), result?.Datanodes?.Count());

            var actualDatanodeReadValue = result?.Datanodes?.First();

            Assert.AreEqual(datanodeReadValue.Name, actualDatanodeReadValue?.Name);
            Assert.AreEqual(datanodeReadValue.Path, actualDatanodeReadValue?.Path);
            Assert.AreEqual(datanodeReadValue.Unit, actualDatanodeReadValue?.Unit);
            Assert.AreEqual(datanodeReadValue.Values.Count(), actualDatanodeReadValue?.Values?.Count());

            var actualDatanodeValueData = result?.Datanodes?.First()?.Values?.First();

            Assert.AreEqual(datanodeValueData.StringValue, actualDatanodeValueData?.StringValue);
            Assert.AreEqual(datanodeValueData.Timestamp, actualDatanodeValueData?.Timestamp);
            
        }

        [TestMethod]
        public async Task TestReadProcessDataAsync_DeviceIdIsNotFound_IotServerCommunicationExceptionIsThrownWithForbiddenStatus()
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
            catch (IoTServerCommunicationException actualException)
            {
                Assert.AreEqual(HttpStatusCode.Forbidden, actualException.HttpStatusCode);
            }
        }

        [TestMethod]
        public async Task TestReadProcessDataAsync_DeviceIdIsNotFound_ErrorInfoDeserializedCorrectly()
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
