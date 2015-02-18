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
    public class ProcessValues
    {
        [DataMember(Name = "href")]
        public Uri Url { get; set; }

        [DataMember(Name = "datanodeReads")]
        public IEnumerable<DatanodeReadValue> Datanodes { get; set; }
    }

    [DataContract]
    public class DatanodeValueData
    {
        public DatanodeReadValue Datanode { get; set; }

        [DataMember(Name = "v")]
        public string StringValue { get; set; }

        public dynamic Value
        {
            get
            {
                var dataType = DataTypeFactory.GetDataType(StringValue, Datanode.DataType);
                return dataType.Value;
            }
        }

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
