using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wapice.IoTTicket.RestClient.Extensions;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public class StatisticalValues
    {
        [DataMember(Name = "href")]
        public Uri Url { get; set; }

        [DataMember(Name = "datanodeReads")]
        public IEnumerable<StatisticalDatanodeReadValue> Datanodes { get; set; }
    }

    [DataContract]
    public class StatisticalDatanodeReadValue : DatanodeBase
    {
        [DataMember(Name = "path", EmitDefaultValue = false)]
        public string Path { get; set; }

        
        [DataMember(Name = "values")]
        public IEnumerable<StatisticalValueData>  Values { get; set; }

    }

    [DataContract]
    public class StatisticalValueData
    {
        [DataMember(Name = "min")]
        public double? Minimum { get; set; }

        [DataMember(Name = "max")]
        public double? Maximum { get; set; }

        [DataMember(Name = "avg")]
        public double? Average { get; set; }

        [DataMember(Name = "count")]
        public uint Count { get; set; }

        [DataMember(Name = "sum")]
        public double Sum { get; set; }

        [IgnoreDataMember]
        public DateTime Timestamp
        {
            get { return UnixTimestamp.FromUnixTime(); }
            set { UnixTimestamp = value.ToUnixTime(); }
        }

        [DataMember(Name = "ts")]
        public long UnixTimestamp { get; set; }
    }
}
