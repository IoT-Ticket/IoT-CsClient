using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public class Quota
    {
        [DataMember(Name = "totalDevices")]
        public int TotalDeviceCount { get; set; }
        [DataMember(Name = "maxNumberOfDevices")]
        public int MaxDeviceCount { get; set; }
        [DataMember(Name = "maxDataNodePerDevice")]
        public int MaxDataNodeCountPerDevice { get; set; }
        [DataMember(Name = "usedStorageSize")]
        public int UsedStorageBytes { get; set; }
        [DataMember(Name = "maxStorageSize")]
        public int MaxStorageBytes { get; set; }
    }
}
