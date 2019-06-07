using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    public class StatisticalDataQueryCriteria
    {
        public StatisticalDataQueryCriteria(string deviceId, Grouping grouping, DateTime startDate, DateTime endDate, params string[] datapoints)
        {
            if (String.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("DeviceId must be set", "deviceId");
            if (datapoints == null || !datapoints.Any())
                throw new ArgumentException("At least one datapoint needs to be defined", "datapoints");

            DeviceId = deviceId;
            Datanodes = datapoints;
            StartDate = startDate;
            EndDate = endDate;
            Grouping = grouping;

        }

        public string DeviceId { get; protected set; }
        public IEnumerable<string> Datanodes { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        // TODO: Should we move Order-enum to own file: would it break existing application using this client?
        public DatanodeQueryCriteria.Order SortOrder { get; set; }
        public Grouping Grouping { get; protected set; }
        public IEnumerable<string> VTags { get; set; } = new List<string>();
    }
}
