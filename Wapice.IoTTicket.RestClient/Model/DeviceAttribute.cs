using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public class DeviceAttribute
    {
        public DeviceAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [DataMember(Name = "key", IsRequired = true)]
        public string Key { get; set; }
        [DataMember(Name = "value", IsRequired = true)]
        public string Value { get; set; }
    }
}
