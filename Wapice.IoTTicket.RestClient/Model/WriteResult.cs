using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public class WriteResult
    {
        [DataMember(Name = "totalWritten")]
        public int TotalWriteCount { get; set; }
        [DataMember(Name = "writeResults")]
        public IEnumerable<DatapointWriteResult> DatapointWriteResults { get; set; }
    }

    [DataContract]
    public class DatapointWriteResult
    {
        [DataMember(Name = "href")]
        public Uri Url { get; set; }
        [DataMember(Name = "writtenCount")]
        public int WriteCount { get; set; } 
    }
}
