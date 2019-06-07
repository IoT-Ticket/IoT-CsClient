using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wapice.IoTTicket.RestClient.Exceptions;
using Wapice.IoTTicket.RestClient.Extensions;
using Wapice.IoTTicket.RestClient.Model;

namespace Wapice.IoTTicket.RestClient
{
    public class IoTTicketClient : IIoTTicketClient, IDisposable
    {
        private const string DevicesResource = "devices/";
        private const string SpecificDeviceResourceFormat = DevicesResource + "{0}/";
        private const string DatanodesResourceFormat = SpecificDeviceResourceFormat + "datanodes/";
        private const string WriteDataResourceFormat = "process/write/{0}/";
        private const string ReadDataResourceFormat = "process/read/{0}/?datanodes={1}";
        private const string ReadStatisticalDataResourceFormat = "stat/read/{0}/?datanodes={1}&fromdate={2}&todate={3}&grouping={4}";
        private const string QuotaAllResource = "quota/all/";
        private const string QuotaDeviceResourceFormat = "quota/{0}/";
        private const string PagedQueryParamsFormat = "?limit={0}&offset={1}";

        private readonly HttpClient _client;

        public IoTTicketClient(Uri baseAddress, string username, SecureString password)
        {
            if (baseAddress == null)
                throw new ArgumentNullException("baseAddress");
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username must be set", "username");
            if (password == null)
                throw new ArgumentException("Password must be set", "password");

            _client = InitHttpClient(baseAddress, new NetworkCredential(username, password));
        }

        private HttpClient InitHttpClient(Uri baseAddress, NetworkCredential credentials)
        {
            var client = new HttpClient(new HttpClientHandler {Credentials = credentials})
            {
                BaseAddress = baseAddress
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("IoTTicket-C#-library", "0.1")));

            return client;
        }

        private HttpContent CreateJsonContent<T>(T data) where T : class
        {
            try
            {
                string json = JsonSerializer.Serialize<T>(data);
                return new StringContent(json, new UTF8Encoding(), "application/json");
            }
            catch (SerializationException ex)
            {
                throw new IoTTicketException("Couldn't serialize the provided data. See inner exception for more details.", ex);
            }
        }

        public async Task<DeviceDetails> RegisterDeviceAsync(Device device, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = CreateJsonContent(device);

            using (var response = await _client.HandledPostAsync(DevicesResource, content, cancellationToken))
            {
                var deviceDetails = await JsonSerializer.DeserializeAsync<DeviceDetails>(response.Content);

                return deviceDetails;
            }
        }

        public async Task<PagedResult<DeviceDetails>> GetDevicesAsync(int count, int skip, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await _client.HandledGetAsync(String.Format(DevicesResource + PagedQueryParamsFormat, count, skip), cancellationToken))
            {
                var pagedResult = await JsonSerializer.DeserializeAsync<PagedResult<DeviceDetails>>(response.Content);

                return pagedResult;
            }
        }

        public async Task<DeviceDetails> GetDeviceAsync(string deviceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("DeviceId must be set", "deviceId");

            using (var response = await _client.HandledGetAsync(String.Format(SpecificDeviceResourceFormat, deviceId), cancellationToken))
            {
                var device = await JsonSerializer.DeserializeAsync<DeviceDetails>(response.Content);

                return device;
            }
        }

        public async Task<PagedResult<DatanodeDetail>> GetDatanodesAsync(string deviceId, int count, int skip, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await _client.HandledGetAsync(
                String.Format(String.Format(DatanodesResourceFormat, deviceId) + PagedQueryParamsFormat, count, skip),
                cancellationToken))
            {
                var pagedResult = await JsonSerializer.DeserializeAsync<PagedResult<DatanodeDetail>>(response.Content);

                return pagedResult;
            }
        }

        public async Task<WriteResult> WriteDatapointAsync(string deviceId, DatanodeWritableValue datapoint, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WriteDatapointCollectionAsync(deviceId, new[] {datapoint}, cancellationToken);
        }

        public async Task<WriteResult> WriteDatapointCollectionAsync(string deviceId, IEnumerable<DatanodeWritableValue> datanodeValues, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = CreateJsonContent(datanodeValues);

            using (var response = await _client.HandledPostAsync(String.Format(WriteDataResourceFormat, deviceId), content, cancellationToken))
            {
                var writeResult = await JsonSerializer.DeserializeAsync<WriteResult>(response.Content);

                return writeResult;
            }
        }

        public async Task<ProcessValues> ReadProcessDataAsync(DatanodeQueryCriteria criteria, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            string uri = String.Format(ReadDataResourceFormat, criteria.DeviceId, Uri.EscapeUriString(String.Join(",", criteria.Datanodes)));

            if (criteria.SortOrder != DatanodeQueryCriteria.Order.Unset)
                uri += String.Format("&order={0}", criteria.SortOrder.ToString().ToLower());

            if (criteria.StartDate.HasValue)
                uri += "&fromdate=" + criteria.StartDate.Value.ToUnixTime();
            if (criteria.EndDate.HasValue)
                uri += "&todate=" + criteria.EndDate.Value.ToUnixTime();
            if (criteria.Count.HasValue)
                uri += "&limit=" + criteria.Count.Value;

            using (var response = await _client.HandledGetAsync(uri, cancellationToken))
            {
                var processValues = await JsonSerializer.DeserializeAsync<ProcessValues>(response.Content);

                return processValues;
            }
        }

        public async Task<StatisticalValues> ReadStatisticalDataAsync(StatisticalDataQueryCriteria criteria, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            string uri = String.Format(ReadStatisticalDataResourceFormat, 
                criteria.DeviceId, 
                Uri.EscapeUriString(String.Join(",", criteria.Datanodes)), 
                criteria.StartDate.ToUnixTime(), 
                criteria.EndDate.ToUnixTime(), 
                criteria.Grouping);

            if (criteria.SortOrder != DatanodeQueryCriteria.Order.Unset)
                uri += String.Format("&order={0}", criteria.SortOrder.ToString().ToLower());
            if (criteria.VTags.Any())
                uri += String.Format("&vtags={0}", String.Join(",", criteria.VTags));

            using (var response = await _client.HandledGetAsync(uri, cancellationToken))
            {
                Console.WriteLine(response.Content.ToString());
                var statisticalValues = await JsonSerializer.DeserializeAsync<StatisticalValues>(response.Content);
                return statisticalValues;
            }
            
        }
        
        public async Task<Quota> GetQuotaAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await _client.HandledGetAsync(QuotaAllResource, cancellationToken))
            {
                var quota = await JsonSerializer.DeserializeAsync<Quota>(response.Content);

                return quota;
            }
        }

        public async Task<DeviceQuota> GetDeviceQuotaAsync(string deviceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await _client.HandledGetAsync(String.Format(QuotaDeviceResourceFormat, deviceId), cancellationToken))
            {
                var deviceQuota = await JsonSerializer.DeserializeAsync<DeviceQuota>(response.Content);

                return deviceQuota;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
