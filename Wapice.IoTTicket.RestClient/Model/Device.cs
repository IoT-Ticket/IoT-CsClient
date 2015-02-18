using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public class Device
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string Type { get; set; }
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }
        [DataMember(Name = "attributes", EmitDefaultValue = false)]
        public IEnumerable<DeviceAttribute> Attributes { get; set; }  
    }

    [DataContract]
    public class DeviceDetails : Device
    {
        [DataMember(Name = "deviceId")]
        public string Id { get; set; }
        [DataMember(Name = "href")]
        public Uri Url { get; set; }
        [DataMember(Name = "createdAt")]
        public DateTime CreationDate { get; set; }
    }
}
