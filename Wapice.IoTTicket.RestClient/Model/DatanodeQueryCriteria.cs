using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    public class DatanodeQueryCriteria
    {
        public enum Order
        {
            Unset = 0,
            Ascending,
            Descending
        }

        public DatanodeQueryCriteria(string deviceId, params string[] datapoints)
        {
            if (String.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("DeviceId must be set", "deviceId");
            if (datapoints == null || !datapoints.Any())
                throw new ArgumentException("At least one datapoint needs to be defined", "datapoints");

            DeviceId = deviceId;
            Datanodes = datapoints;
        }

        public string DeviceId { get; protected set; }
        public IEnumerable<string> Datanodes { get; protected set; }
        public int? Count { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Order SortOrder { get; set; }
    }
}
