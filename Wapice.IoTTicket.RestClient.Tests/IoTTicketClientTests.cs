using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wapice.IoTTicket.RestClient.Model;

namespace Wapice.IoTTicket.RestClient.Tests
{
    [TestClass]
    public class IoTTicketClientTests
    {
        private IIoTTicketClient _client;

        private const string Username = "<user_name>";
        private const string Password = "<password>";
        private const string DeviceId = "<your_device_id>"; // se instructions below

        [TestInitialize]
        public void InitializeTest()
        {
            var pwd = new SecureString();
            Password.ToList().ForEach(pwd.AppendChar);

            _client = new IoTTicketClient(new Uri("https://my.iot-ticket.com/api/v1/"), Username, pwd);
        }

        /// <summary>
        /// How to setup the unit tests:
        /// 1. Register your own account at https://my.iot-ticket.com/Dashboard/
        /// 2. Add your credentials to the constants above
        /// 3. Run this test once (temporarly comment the [Ignore] attribute) to create a test device. 
        ///    The id will be written to 'Wapice.IoTTicket.RestClient.Tests\bin\(debug|release)\TestDeviceId.txt'.
        /// 4. Disable this test again by uncommenting the [Ignore] attribute.
        /// 5. Run the other tests.
        /// </summary>
        [TestMethod]
        [Ignore]
        public void RegisterDevice_ValidDeviceObjectAndNoCancellationToken_Accepted()
        {
            var device = new Device
            {
                Name = "MyTestDevice",
                Description = "Testing my device",
                Manufacturer = "My",
                Type = "PC",
                Attributes =
                    new List<DeviceAttribute>
                    {
                        new DeviceAttribute("testKey1", "testValue1"),
                        new DeviceAttribute("testKey2", "testValue2")
                    }
            };

            var deviceDetails = _client.RegisterDeviceAsync(device).Result;

            Assert.IsNotNull(deviceDetails);
            Assert.AreNotEqual(DateTime.MinValue, deviceDetails.CreationDate);
            Assert.AreNotEqual(String.Empty, deviceDetails.Id);
            Assert.IsNotNull(deviceDetails.Url);

            File.WriteAllText("TestDeviceId.txt", deviceDetails.Id);
        }

        [TestMethod]
        public void WriteDatapoint_IntegerValueAndNoCancellationToken_Accepted()
        {
            var datapoint = new DatanodeWritableValue
            {
                Name = "MyTestDatapoint",
                Path = "My/Test",
                Value = new Random().Next(100),
                Unit = "%",
                Timestamp = DateTime.Now.AddMinutes(-1)
            };

            var writeResult = _client.WriteDatapointAsync(DeviceId, datapoint).Result;

            Assert.IsNotNull(writeResult);
            Assert.AreEqual(1, writeResult.TotalWriteCount);
            Assert.IsNotNull(writeResult.DatapointWriteResults);
            Assert.IsTrue(writeResult.DatapointWriteResults.Any());
            Assert.AreEqual(1, writeResult.DatapointWriteResults.First().WriteCount);
            Assert.IsNotNull(writeResult.DatapointWriteResults.First().Url);
        }

        [TestMethod]
        public void WriteDatapoint_ByteArrayValueAndNoCancellationToken_Accepted()
        {
            var datapoint = new DatanodeWritableValue
            {
                Name = "MyByteTestDatapoint",
                Path = "My/Test",
                Value = new byte[] { 1, 2, 3, 4, 5, 6,7, 9, 10},
                Unit = "byte",
            };

            var writeResult = _client.WriteDatapointAsync(DeviceId, datapoint).Result;

            Assert.IsNotNull(writeResult);
            Assert.AreNotEqual(0, writeResult.TotalWriteCount);
            Assert.IsNotNull(writeResult.DatapointWriteResults);
            Assert.IsTrue(writeResult.DatapointWriteResults.Any());
            Assert.AreNotEqual(0, writeResult.DatapointWriteResults.First().WriteCount);
            Assert.IsNotNull(writeResult.DatapointWriteResults.First().Url);
        }

        [TestMethod]
        public void WriteDatapoint_BooleanValueAndNoCancellationToken_Accepted()
        {
            var datapoint = new DatanodeWritableValue
            {
                Name = "MyBooleanTestDatapoint",
                Path = "My/Test",
                Value = true,
            };

            var writeResult = _client.WriteDatapointAsync(DeviceId, datapoint).Result;

            Assert.IsNotNull(writeResult);
            Assert.AreNotEqual(0, writeResult.TotalWriteCount);
            Assert.IsNotNull(writeResult.DatapointWriteResults);
            Assert.IsTrue(writeResult.DatapointWriteResults.Any());
            Assert.AreNotEqual(0, writeResult.DatapointWriteResults.First().WriteCount);
            Assert.IsNotNull(writeResult.DatapointWriteResults.First().Url);
        }

        [TestMethod]
        public void ReadProcessData_NoCancellationToken_ResultsReturned()
        {
            var criteria = new DatanodeQueryCriteria(DeviceId, "MyTestDatapoint")
            {
                Count = 5,
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now,
                SortOrder = DatanodeQueryCriteria.Order.Descending
            };

            var processData = _client.ReadProcessDataAsync(criteria).Result;

            Assert.IsNotNull(processData);
            Assert.IsNotNull(processData.Datanodes);
            Assert.IsNotNull(processData.Datanodes.First());
            Assert.IsTrue(processData.Datanodes.First().Values.Any());
            Assert.AreNotEqual(DateTime.MinValue, processData.Datanodes.First().Values.First().Timestamp);
            Assert.AreNotEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), processData.Datanodes.First().Values.First().Timestamp);
            Assert.IsNotNull(DateTime.MinValue, processData.Datanodes.First().Values.First().StringValue);
        }

        [TestMethod]
        public void ReadProcessData_CancellationTokenSet_TaskCancelled()
        {
            var cancellationToken = new CancellationTokenSource(10).Token;
            var task = _client.ReadProcessDataAsync(new DatanodeQueryCriteria(DeviceId, "cpu load"), cancellationToken);

            TestTaskCancellation(task, cancellationToken);
        }

        [TestMethod]
        public void GetDatanodes_NoCancellationToken_ResultsReturned()
        {
            var pagedResult = _client.GetDatanodesAsync(DeviceId, 10, 0).Result;

            Assert.IsNotNull(pagedResult);
            Assert.IsNotNull(pagedResult.Result);
            Assert.IsTrue(pagedResult.RequestedCount > 0);
            Assert.IsTrue(pagedResult.Result.Any());
        }

        [TestMethod]
        public void GetDatanodes_CancellationTokenSet_TaskCancelled()
        {
            var cancellationToken = new CancellationTokenSource(10).Token;
            var task = _client.GetDatanodesAsync(DeviceId, 10, 0, cancellationToken);

            TestTaskCancellation(task, cancellationToken);
        }

        [TestMethod]
        public void GetDevices_NoCancellationToken_ResultsReturned()
        {
            var pagedResult = _client.GetDevicesAsync(10, 0).Result;

            Assert.IsNotNull(pagedResult);
            Assert.IsNotNull(pagedResult.Result);
            Assert.IsTrue(pagedResult.RequestedCount > 0);
            Assert.IsTrue(pagedResult.Result.Any());
        }

        [TestMethod]
        public void GetDevices_CancellationTokenSet_TaskCancelled()
        {
            var cancellationToken = new CancellationTokenSource(10).Token;
            var task = _client.GetDevicesAsync(10, 0, cancellationToken);

            TestTaskCancellation(task, cancellationToken);
        }
        
        [TestMethod]
        public void GetDevice_NoCancellationToken_ResultsReturned()
        {
            var device = _client.GetDeviceAsync(DeviceId).Result;

            Assert.IsNotNull(device);
            Assert.AreEqual(DeviceId, device.Id);
            Assert.AreNotEqual(new DateTime(), device.CreationDate);
        }

        [TestMethod]
        public void GetDevice_CancellationTokenSet_TaskCancelled()
        {
            var cancellationToken = new CancellationTokenSource(10).Token;
            var task = _client.GetDeviceAsync(DeviceId, cancellationToken);

            TestTaskCancellation(task, cancellationToken);
        }

        [TestMethod]
        public void GetQuota_NoCancellationToken_ResultsReturned()
        {
            var quota = _client.GetQuotaAsync().Result;

            Assert.IsNotNull(quota);
        }

        [TestMethod]
        public void GetQuota_CancellationTokenSet_TaskCancelled()
        {
            var cancellationToken = new CancellationTokenSource(10).Token;
            var task = _client.GetQuotaAsync(cancellationToken);

            TestTaskCancellation(task, cancellationToken);
        }

        [TestMethod]
        public void GetDeviceQuota_CancellationTokenSet_TaskCancelled()
        {
            var cancellationToken = new CancellationTokenSource(10).Token;
            var task = _client.GetDeviceQuotaAsync(DeviceId, cancellationToken);

            TestTaskCancellation(task, cancellationToken);
        }

        [TestMethod]
        public void GetDeviceQuota_NoCancellationToken_ResultsReturned()
        {
            var deviceQuota = _client.GetDeviceQuotaAsync(DeviceId).Result;

            Assert.IsNotNull(deviceQuota);
        }

        [TestMethod]
        public void ReadStatisticalData_NoCancellationToken_ResultsReturned()
        {

            DateTime startDate = DateTime.Now.AddHours(-1);
            DateTime endDate = DateTime.Now;

            StatisticalDataQueryCriteria criteria = new StatisticalDataQueryCriteria(DeviceId, StatisticalDataQueryCriteria.Grouping.Day, startDate, endDate, "MyTestDataPoint");

            var statisticalValues = _client.ReadStatisticalDataAsync(criteria).Result;
            Assert.IsNotNull(statisticalValues);
            Assert.IsNotNull(statisticalValues.Url);
            Assert.IsNotNull(statisticalValues.Datanodes);

            Assert.AreEqual(1, statisticalValues.Datanodes.Count());
            StatisticalDatanodeReadValue datanodeReadValue = statisticalValues.Datanodes.First();

            Assert.AreEqual("MyTestDatapoint", datanodeReadValue.Name);
            Assert.AreEqual("My/Test", datanodeReadValue.Path);
            Assert.AreEqual(LongDataType.TypeName, datanodeReadValue.DataType);
            Assert.AreEqual("%", datanodeReadValue.Unit);

            Assert.AreEqual(1, datanodeReadValue.Values.Count());
            StatisticalValueData valueData = datanodeReadValue.Values.First();

            Assert.AreNotEqual(DateTime.MinValue, statisticalValues.Datanodes.First().Values.First().UnixTimestamp);
            Assert.AreNotEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), statisticalValues.Datanodes.First().Values.First().Count);
            Assert.AreNotEqual(0u, valueData.Count);
            Assert.IsNotNull(valueData.Minimum);
            Assert.IsNotNull(valueData.Maximum);
            Assert.IsNotNull(valueData.Average);
            }

        [TestMethod]
        public void ReadStatisticalData_NoCancellationTokenAndNoStatisticalDataFound_ResultsReturned()
        {
            DateTime startDate = DateTime.Now.AddYears(10).AddHours(-1);
            DateTime endDate = DateTime.Now.AddYears(10);

            StatisticalDataQueryCriteria criteria = new StatisticalDataQueryCriteria(DeviceId, StatisticalDataQueryCriteria.Grouping.Day, startDate, endDate, "MyTestDataPoint");

            var statisticalValues = _client.ReadStatisticalDataAsync(criteria).Result;
            Assert.IsNotNull(statisticalValues);
            Assert.IsNotNull(statisticalValues.Url);
            Assert.IsNotNull(statisticalValues.Datanodes);

            Assert.AreEqual(1, statisticalValues.Datanodes.Count());
            StatisticalDatanodeReadValue datanodeReadValue = statisticalValues.Datanodes.First();

            Assert.AreEqual("MyTestDatapoint", datanodeReadValue.Name);
            Assert.AreEqual("My/Test", datanodeReadValue.Path);
            Assert.AreEqual(LongDataType.TypeName, datanodeReadValue.DataType);
            Assert.AreEqual("%", datanodeReadValue.Unit);

            Assert.AreEqual(1, datanodeReadValue.Values.Count());
            StatisticalValueData valueData = datanodeReadValue.Values.First();

            Assert.AreEqual(0d, valueData.Sum);
            Assert.AreEqual(0u, valueData.Count);
            Assert.IsNull(valueData.Minimum);
            Assert.IsNull(valueData.Maximum);
            Assert.IsNull(valueData.Average);
            Assert.AreNotEqual(DateTime.MinValue, statisticalValues.Datanodes.First().Values.First().UnixTimestamp);
            Assert.AreNotEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), statisticalValues.Datanodes.First().Values.First().Count);
        }

        private void TestTaskCancellation(Task task, CancellationToken cancellationToken)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException is TaskCanceledException)
                {
                    Assert.IsTrue(task.IsCanceled);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
