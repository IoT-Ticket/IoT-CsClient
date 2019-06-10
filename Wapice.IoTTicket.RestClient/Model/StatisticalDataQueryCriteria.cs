using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{

    public class StatisticalDataQueryCriteria
    {
        public enum Grouping
        {
            Minute,
            Hour,
            Day,
            Week,
            Month,
            Year
        }

        public StatisticalDataQueryCriteria(string deviceId, Grouping valueGrouping, DateTime startDate, DateTime endDate, params string[] datapoints)
        {
            if (String.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("DeviceId must be set", nameof(deviceId));
            if (datapoints == null || !datapoints.Any())
                throw new ArgumentException("At least one datapoint needs to be defined", nameof(datapoints));
            if ( !(startDate < endDate) )
            {
                throw new ArgumentException(String.Format("{0} must be before {1}", nameof(startDate), nameof(endDate)));
            }

            DeviceId = deviceId;
            Datanodes = datapoints;
            StartDate = startDate;
            EndDate = endDate;
            ValueGrouping = valueGrouping;

        }

        public string DeviceId { get; protected set; }
        public IEnumerable<string> Datanodes { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public DatanodeQueryCriteria.Order SortOrder { get; set; }
        public Grouping ValueGrouping { get; protected set; }
        // TODO: Ask about what to do with vtags
        public IEnumerable<string> VTags { get; set; } = new List<string>();
    }
}
