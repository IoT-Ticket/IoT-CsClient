using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public class DeviceQuota
    {
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }
        [DataMember(Name = "totalRequestToday")]
        public int RequestCountToday { get; set; }
        [DataMember(Name = "maxReadRequestPerDay")]
        public int MaxReadRequestCountPerDay { get; set; }
        [DataMember(Name = "numberOfDataNodes")]
        public int DataNodeCount { get; set; }
        [DataMember(Name = "storageSize")]
        public int UsedStorageBytes { get; set; }
    }
}
