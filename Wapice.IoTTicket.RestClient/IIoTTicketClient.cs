using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wapice.IoTTicket.RestClient.Model;

namespace Wapice.IoTTicket.RestClient
{
    public interface IIoTTicketClient
    {
        /// <summary>
        /// Registers a device on the server.
        /// </summary>
        /// <param name="device">Device to register.</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>The registered device details.</returns>
        Task<DeviceDetails> RegisterDeviceAsync(Device device, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Query the user's device collection with paging support.
        /// </summary>
        /// <param name="count">Max result count to fetch.</param>
        /// <param name="skip">Amount to skip (offset).</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>A <see cref="PagedResult{DeviceDetails}"/> of <see cref="DeviceDetails"/>.</returns>
        Task<PagedResult<DeviceDetails>> GetDevicesAsync(int count, int skip, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets a device's details
        /// </summary>
        /// <param name="deviceId">The id of the device.</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>The device details.</returns>
        Task<DeviceDetails> GetDeviceAsync(string deviceId, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Query a device's datanodes with paging support.
        /// </summary>
        /// <param name="deviceId">The device to query.</param>
        /// <param name="count">Max result count to fetch.</param>
        /// <param name="skip">Amount to skip (offset).</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>A <see cref="PagedResult{DatanodeDetails}"/> of <see cref="DeviceDetails"/>.</returns>
        Task<PagedResult<DatanodeDetail>> GetDatanodesAsync(string deviceId, int count, int skip, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Writes a single process value to a datapoint on a device.
        /// </summary>
        /// <param name="deviceId">The device to which the datapoint belongs.</param>
        /// <param name="datanodeValue">The value to write.</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>The result of the write.</returns>
        Task<WriteResult> WriteDatapointAsync(string deviceId, DatanodeWritableValue datanodeValue, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Writes a collection of datanode process values to a device.
        /// </summary>
        /// <param name="deviceId">The device to which the datapoint belongs.</param>
        /// <param name="datanodeValues">A collection of <see cref="DatanodeWritableValue"/> to write.</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>The result of the write.</returns>
        Task<WriteResult> WriteDatapointCollectionAsync(string deviceId, IEnumerable<DatanodeWritableValue> datanodeValues, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Reads process values using a query object. <see cref="DatanodeQueryCriteria"/> for more information.
        /// </summary>
        /// <param name="criteria">The criteria to query the server with.</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>The result of the query. See <see cref="ProcessValues"/> for more information.</returns>
        Task<ProcessValues> ReadProcessDataAsync(DatanodeQueryCriteria criteria, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Reads statistical data using a query object. <see cref="StatisticalDataQueryCriteria"/> for more information.
        /// </summary>
        /// <param name="criteria">The criteria to query the server with.</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>The result of the query. See <see cref="StatisticalValues"/> for more information.</returns>
        Task<StatisticalValues> ReadStatisticalDataAsync(StatisticalDataQueryCriteria criteria, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get user's root enterprises from the server with paging support. 
        /// </summary>
        /// <param name="count">Max result count to fetch.</param>
        /// <param name="skip">Amount to skip (offset).</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>A <see cref="PagedResult{Enterprise}"/> of <see cref="Enterprise"/>.</returns>
        Task<PagedResult<Enterprise>> GetRootEnterpricesAsync(int count, int skip, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get sub enterprises under the enterprise with the provided <paramref name="enterpriseId"/> from the server with paging support. 
        /// </summary>
        /// <param name="enterpriseId">The resource id of parent Enterprise.</param>
        /// <param name="count">Max result count to fetch.</param>
        /// <param name="skip">Amount to skip (offset).</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>A <see cref="PagedResult{Enterprise}"/> of <see cref="Enterprise"/>.</returns>
        Task<PagedResult<Enterprise>> GetSubEnterpricesAsync(string enterpriseId, int count, int skip, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the user's quota information from the server.
        /// </summary>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns>The quota</returns>
        Task<Quota> GetQuotaAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets the defined device's quota information from the server.
        /// </summary>
        /// <param name="deviceId">The device to query quota for.</param>
        /// <param name="cancellationToken">An optional cancellation token to cancel the request.</param>
        /// <returns></returns>
        Task<DeviceQuota> GetDeviceQuotaAsync(string deviceId, CancellationToken cancellationToken = default(CancellationToken));
    }
}