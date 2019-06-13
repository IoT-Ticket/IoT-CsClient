using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
        public void TestRegisterDeviceAsync()
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

            var device = new Device
            {
                Name = DEVICE_NAME,
                Type = DEVICE_TYPE,
                Attributes = deviceAttributes,
                Description = DEVICE_DESCRIPTION,
                Manufacturer = DEVICE_MANUFACTURER
            };

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
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(deviceDetails)));

            var result = Client.RegisterDeviceAsync(device).Result;
            Assert.IsNotNull(result);

            AssertReceivedAsOnlyRequest(expectedRequest);
        }
    }
}
