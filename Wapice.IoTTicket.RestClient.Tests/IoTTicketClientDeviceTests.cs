using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Wapice.IoTTicket.RestClient.Exceptions;
using Wapice.IoTTicket.RestClient.Model;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Wapice.IoTTicket.RestClient.Tests
{

    [TestClass]
    public class IoTTicketClientDeviceTests: WireMockTestBase
    {

        DateTime DEVICE_CREATED_AT = new DateTime(2019, 1, 1, 0, 0, 0, 0);
        const string DEVICE_DEVICE_ID = "deviceId";
        const string DEVICE_HREF = "https://my.iot-ticket.com/api/v1/devices/b3076fd2dd514397a19fd24fd07bf7e1";
        const string DEVICE_NAME = "name";
        const string DEVICE_MANUFACTURER = "manufacturer";
        const string DEVICE_TYPE = "type";
        const string DEVICE_DESCRIPTION = "description";

        const string DEVICE_ATTRIBUTE_KEY = "key";
        const string DEVICE_ATTRIBUTE_VALUE = "value";

        [TestMethod]
        public void TestRegisterDeviceAsync_ValidDevice_DeviceDetailsReturnedSuccessfully()
        {
            Device device = CreateValidDevice();
            DeviceDetails deviceDetails = CreateValidDeviceDetails();

            var expectedRequest =
                Request
                .Create()
                .WithPath("/devices/")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher(JsonSerializer.Serialize(device)))
                .UsingPost();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(deviceDetails)));

            var result = Client.RegisterDeviceAsync(device).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public async Task TestRegisterDeviceAsync_MissingManufacturerField_IoTServerCommunicationExceptionIsThrown()
        {
            var deviceWithMissingManufacturer = new Device
            {
                Name = "name"
            };

            var errorInfo = new ErrorInfo
            {
                Description = "DeviceManufacturer is needed",
                Code = 8003,
                MoreInfoUrl = new Uri("https://my.iot-ticket.com/api/v1/errorcodes"),
                ApiVersion = 1
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/devices/")
                .WithHeader("Accept", "application/json")
                .WithBody(new JsonMatcher(JsonSerializer.Serialize(deviceWithMissingManufacturer)))
                .UsingPost();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(errorInfo)));

            try
            {
                await Client.RegisterDeviceAsync(deviceWithMissingManufacturer);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException expectedException)
            {
                Assert.IsNotNull(expectedException.ErrorInfo);
                Assert.AreEqual(HttpStatusCode.BadRequest, expectedException.HttpStatusCode);
            }

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public void TestGetDevicesAsync_DevicesFound_ReturnDevicesSuccessfully()
        {
            const int LIMIT = 2;
            const int OFFSET = 1;
            DeviceDetails deviceDetails = CreateValidDeviceDetails();

            PagedResult<DeviceDetails> response = new PagedResult<DeviceDetails>()
            {
                Result = new List<DeviceDetails>
                {
                    deviceDetails
                }
            };

            var expectedRequest =
                Request
                .Create()
                .WithPath("/devices/")
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
                .WithBody(JsonSerializer.Serialize(deviceDetails)));

            var result = Client.GetDevicesAsync(LIMIT, OFFSET).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);

        }

        [TestMethod]
        public void TestGetDeviceAsync_DeviceExists_DeviceDetailsReturnedSuccessfully()
        {
            DeviceDetails deviceDetails = CreateValidDeviceDetails();
            var deviceId = deviceDetails.Id;

            var expectedRequest =
                Request
                .Create()
                .WithPath("/devices/" + deviceId + "/")
                .WithHeader("Accept", "application/json")
                .UsingGet();

            Server
                .Given(expectedRequest)
                .RespondWith(Response
                .Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(deviceDetails)));

            var result = Client.GetDeviceAsync(deviceId).Result;

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        [TestMethod]
        public async Task TestGetDeviceAsync_DeviceIsNotFound_IoTServerCommunicationExceptionIsThrown()
        {
            const string NOT_EXISTING_DEVICE_ID = "id2";

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
                .WithPath("/devices/" + NOT_EXISTING_DEVICE_ID + "/")
                .WithHeader("Accept", "application/json")
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
                await Client.GetDeviceAsync(NOT_EXISTING_DEVICE_ID);
                Assert.Fail("Expected " + nameof(IoTServerCommunicationException));
            }
            catch (IoTServerCommunicationException exceptedException)
            {
                Assert.IsNotNull(exceptedException);
                Assert.AreEqual(HttpStatusCode.Forbidden, exceptedException.HttpStatusCode);
            }

            AssertReceivedAsOnlyRequest(expectedRequest);
        }

        private DeviceDetails CreateValidDeviceDetails()
        {
            var deviceAttributes = new List<DeviceAttribute>
            {
                new DeviceAttribute(DEVICE_ATTRIBUTE_KEY, DEVICE_ATTRIBUTE_VALUE)
            };

            var deviceDetails = new DeviceDetails
            {
                CreationDate = DEVICE_CREATED_AT,
                Id = DEVICE_DEVICE_ID,
                Url = new Uri(DEVICE_HREF),
                Attributes = deviceAttributes,
                Manufacturer = DEVICE_MANUFACTURER,
                Name = DEVICE_NAME
            };
            return deviceDetails;
        }

        private Device CreateValidDevice()
        {
            var deviceAttributes = new List<DeviceAttribute>
            {
                new DeviceAttribute(DEVICE_ATTRIBUTE_KEY, DEVICE_ATTRIBUTE_VALUE)
            };


            var device = new Device
            {
                Name = DEVICE_NAME,
                Type = DEVICE_TYPE,
                Attributes = deviceAttributes,
                Description = DEVICE_DESCRIPTION,
                Manufacturer = DEVICE_MANUFACTURER
            };

            return device;
        }

    }
}
